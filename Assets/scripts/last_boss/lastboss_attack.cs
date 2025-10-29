using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Lastboss_Attack : MonoBehaviour
{
    [Header("攻撃フェーズの設定")]
    [Tooltip("この時間（秒）が経過すると、ボスは新しい攻撃をしなくなります")]
    public float totalAttackDuration = 108f; // 例: 108秒間攻撃を続ける

    [Header("点滅攻撃の設定")]
    public float blinkingAttackInterval = 10f;
    public Texture2D changeTexture;
    public Texture2D changeEmissionTexture;
    public Color changeEmissionColor = Color.white;
    public float changeDuration = 3f;
    public int requiredHits = 3;
    public int scoreOnSuccess = 100;
    public float blinkInterval = 0.2f;
    public float fastBlinkInterval = 0.05f;

    [Header("無敵化攻撃の設定")]
    public float invincibleAttackInterval = 15f;
    public Texture2D invincibleTexture;
    public Texture2D invincibleEmissionTexture;
    public Color invincibleEmissionColor = Color.red;
    public float invincibleDuration = 4f;

    [Header("パーティクル設定")]
    public ParticleSystem successParticle;
    public ParticleSystem failureParticle;

    [Header("共通設定")]
    public string targetTag = "monitor";
    public int scorePenaltyOnCharge = 50;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.1f;

    private class AttackInfo
    {
        public Texture originalTexture;
        public Texture originalEmissionTexture;
        public Color originalEmissionColor;
        public Coroutine coroutine;
        public bool isAttackSuccessful = false; // 攻撃が成功したかどうかを追跡
    }

    private HashSet<TargetObject> activeMonitors = new HashSet<TargetObject>();
    private Dictionary<TargetObject, int> blinkingHitCounts = new Dictionary<TargetObject, int>();
    private Dictionary<TargetObject, AttackInfo> runningAttacks = new Dictionary<TargetObject, AttackInfo>();

    void Start()
    {
        InvokeRepeating("TryStartBlinkingAttack", 5f, blinkingAttackInterval);
        InvokeRepeating("TryStartInvincibleAttack", 10f, invincibleAttackInterval);

        Invoke("StopAllAttacks", totalAttackDuration);
    }

    public void TryStartBlinkingAttack()
    {
        StartAttack(AttackType.Blinking);
    }

    public void TryStartInvincibleAttack()
    {
        StartAttack(AttackType.Invincible);
    }

    private void StartAttack(AttackType type)
    {
        List<TargetObject> availableMonitors = GameObject.FindGameObjectsWithTag(targetTag)
            .Select(go => go.GetComponent<TargetObject>())
            .Where(t => t != null && !activeMonitors.Contains(t))
            .ToList();

        if (availableMonitors.Count == 0) return;

        TargetObject target = availableMonitors[Random.Range(0, availableMonitors.Count)];
        Material material = target.GetComponent<Renderer>().material;

        activeMonitors.Add(target);

        var attackInfo = new AttackInfo
        {
            originalTexture = material.mainTexture,
            originalEmissionTexture = material.GetTexture("_EmissionMap"),
            originalEmissionColor = material.GetColor("_EmissionColor")
        };

        if (type == AttackType.Blinking)
        {
            blinkingHitCounts[target] = 0;
            attackInfo.coroutine = StartCoroutine(BlinkRoutine(target, material, attackInfo));
        }
        else if (type == AttackType.Invincible)
        {
            attackInfo.coroutine = StartCoroutine(InvincibleRoutine(target, material, attackInfo));
        }
        
        if (attackInfo.coroutine != null)
        {
            runningAttacks[target] = attackInfo;
        }
    }

    private IEnumerator BlinkRoutine(TargetObject target, Material material, AttackInfo info)
    {
        target.isInvulnerable = true;
        float endTime = Time.time + changeDuration;
        bool isBlinkingOn = true;

        while (Time.time < endTime)
        {
            // オブジェクトが破棄されたか、攻撃が成功したらコルーチンを停止
            if (target == null || info.isAttackSuccessful)
            {
                yield break;
            }

            if (isBlinkingOn)
            {
                material.mainTexture = changeTexture;
                material.EnableKeyword("_EMISSION");
                material.SetTexture("_EmissionMap", changeEmissionTexture);
                material.SetColor("_EmissionColor", changeEmissionColor);
            }
            else
            {
                material.mainTexture = info.originalTexture;
                if (info.originalEmissionTexture != null)
                {
                    material.SetTexture("_EmissionMap", info.originalEmissionTexture);
                    material.SetColor("_EmissionColor", info.originalEmissionColor);
                }
                else
                {
                    material.DisableKeyword("_EMISSION");
                }
            }
            
            isBlinkingOn = !isBlinkingOn;
            float currentBlinkInterval = (endTime - Time.time <= 1f) ? fastBlinkInterval : blinkInterval;
            yield return new WaitForSeconds(currentBlinkInterval);
        }

        // タイムアウト時にオブジェクトがまだ存在し、攻撃が成功していなければ失敗処理を実行
        if (target != null && !info.isAttackSuccessful)
        {
            if (failureParticle != null)
            {
                Instantiate(failureParticle, target.transform.position, Quaternion.identity);
            }
            HandlePenalty();
            RestoreTexture(target, material, info);
        }
    }

    private IEnumerator InvincibleRoutine(TargetObject target, Material material, AttackInfo info)
    {
        target.isInvulnerable = true;
        material.mainTexture = invincibleTexture;
        material.EnableKeyword("_EMISSION");
        material.SetTexture("_EmissionMap", invincibleEmissionTexture);
        material.SetColor("_EmissionColor", invincibleEmissionColor);

        yield return new WaitForSeconds(invincibleDuration);

        RestoreTexture(target, material, info);
    }

    public void OnTargetHitByBullet(TargetObject hitObject, int player_num)
    {
        if (blinkingHitCounts.ContainsKey(hitObject))
        {
            blinkingHitCounts[hitObject]++;

            int playerCount = Mathf.Max(1, player_manager.player_count);
            int currentRequiredHits = requiredHits;

            Debug.Log($"Special target {hitObject.name} hit! Hit count: {blinkingHitCounts[hitObject]}");

            if (blinkingHitCounts[hitObject] >= currentRequiredHits)
            {
                if (runningAttacks.TryGetValue(hitObject, out AttackInfo info))
                {
                    // 攻撃が成功したことをマーク
                    info.isAttackSuccessful = true;

                    Debug.Log($"Attack on {hitObject.name} successful!");
                    if (ScoreManager.instance != null)
                    {
                        ScoreManager.instance.AddScore(player_num, scoreOnSuccess);
                    }
                    if (successParticle != null)
                    {
                        Instantiate(successParticle, hitObject.transform.position, Quaternion.identity);
                    }
                
                    StopCoroutine(info.coroutine);
                    Material material = hitObject.GetComponent<Renderer>().material;
                    RestoreTexture(hitObject, material, info);
                }
            }
        }
    }
    
    private void RestoreTexture(TargetObject target, Material material, AttackInfo info)
    {
        material.mainTexture = info.originalTexture;
        material.SetTexture("_EmissionMap", info.originalEmissionTexture);
        material.SetColor("_EmissionColor", info.originalEmissionColor);
        if (info.originalEmissionTexture == null)
        {
            material.DisableKeyword("_EMISSION");
        }

        target.isInvulnerable = false;
        activeMonitors.Remove(target);
        blinkingHitCounts.Remove(target);
        runningAttacks.Remove(target);
    }

    private void HandlePenalty()
    {
        Debug.Log("Attack failed! Score decreased.");
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
        }
        if (ScoreManager.instance != null)
        {
            for (int i = 0; i < ScoreManager.scores.Length; i++)
            {
                ScoreManager.instance.DecreaseScore(i, scorePenaltyOnCharge);
            }
        }
    }

    public bool IsSpecialAttackTarget(TargetObject target)
    {
        return activeMonitors.Contains(target);
    }

    public void StopAllAttacks()
    {
        Debug.Log("ボスの攻撃フェーズが終了。全ての攻撃を停止します。");

        // 1. InvokeRepeating での新規の攻撃発生を停止
        CancelInvoke("TryStartBlinkingAttack");
        CancelInvoke("TryStartInvincibleAttack");

        // 2. 現在実行中の攻撃（コルーチン）をすべて停止・復元
        //    辞書をコピーしてループします（ループ内で元の辞書を変更するため）
        List<TargetObject> targetsToStop = runningAttacks.Keys.ToList();

        foreach (TargetObject target in targetsToStop)
        {
            // ターゲットが（破壊などで）既に null (破棄済み) でないかチェック
            if (target != null) 
            {
                if (runningAttacks.TryGetValue(target, out AttackInfo info))
                {
                    if (info.coroutine != null)
                    {
                        // 実行中のコルーチンを停止
                        StopCoroutine(info.coroutine);
                    }
                    
                    Renderer renderer = target.GetComponent<Renderer>();
                    if (renderer != null && renderer.material != null)
                    {
                        Material material = renderer.material;
                        // テクスチャや状態を元に戻す
                        RestoreTexture(target, material, info);
                    }
                    else
                    {
                        // レンダラーがない場合でも、管理リストからは削除する
                        activeMonitors.Remove(target);
                        blinkingHitCounts.Remove(target);
                        runningAttacks.Remove(target);
                    }
                }
            }
        }
        
        // 強制停止なので、リストに残骸が残らないように全てクリアします
        runningAttacks.Clear();
        activeMonitors.Clear();
        blinkingHitCounts.Clear();
    }

    private enum AttackType { Blinking, Invincible }
}