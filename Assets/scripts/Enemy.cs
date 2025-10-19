using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class Enemy : MonoBehaviour
{
    [Header("基本設定")]
    public int maxHP = 10; // HP追加
    public int scoreValue = 10;
    public AudioClip explodeSound; // 爆発音
    public AudioClip damageSound; // ダメージ時の効果音を追加
    public GameObject breakEffect;
    public Vector3 breakEffectOffset = new Vector3(0, 1f, 0); // オフセット追加

    [Header("突進関連")]
    // Inspector で最小/最大を指定してその範囲から乱数で待機時間を決める
    public float waitBeforeCharge_min = 1.0f; // 突進前の最小待機秒数
    public float waitBeforeCharge_max = 3.0f; // 突進前の最大待機秒数
    private float chargeDistance = -1.0f; // 突進する距離
    private float chargeSpeed = 30.0f; // 突進速度
    private float returnSpeed = 30.0f; // 戻る速度
    public int scorePenaltyOnCharge = 50; // 突進時に減らすスコア
    public float shakeDuration = 0.3f; // カメラを揺らす時間
    public float shakeMagnitude = 0.1f; // カメラを揺らす強さ

    public event Action OnDeath; // 死亡時に呼ばれるイベント

    private Vector3 originalPosition; // 元の座標
    private bool isCharging = false;
    private bool isPreparing = false; // 突進の準備中フラグ
    private bool chargeCanceled = false; // 準備中にキャンセルされたか
    public float prepareDuration = 2.0f; // 突進開始前の予備時間（プレイヤーが攻撃してキャンセルできる）
    private int currentHP;
    private MonoBehaviour[] otherScripts; // 自分以外のスクリプト
    [Header("Cancel Effects")]
    [Tooltip("突進がキャンセルされたときに再生するエフェクト（プレハブ）")]
    public GameObject cancelEffectPrefab;
    [Tooltip("突進がキャンセルされたときに再生するサウンド")]
    public AudioClip cancelSound;

    void Start()
    {
        currentHP = maxHP;
        originalPosition = transform.position;

        // 自分以外の有効なMonoBehaviourを取得
        otherScripts = GetComponents<MonoBehaviour>().Where(script => script != this && script.enabled).ToArray();

        // min/max の整合性チェック（min が max より大きければ入れ替える）
        if (waitBeforeCharge_min > waitBeforeCharge_max)
        {
            float tmp = waitBeforeCharge_min;
            waitBeforeCharge_min = waitBeforeCharge_max;
            waitBeforeCharge_max = tmp;
        }

        // 突進コルーチンを開始
        StartCoroutine(ChargeRoutine());
    }

    // ダメージを受ける
    public void TakeDamage(int damage, int player_num)
    {
        currentHP -= damage;
        // 準備中にダメージを受けたら突進をキャンセル
        if (isPreparing)
        {
            CancelCharge();
        }
        // ボス用の HP 管理があればそちらに委譲して重複処理を防ぐ
        var bossHp = GetComponentInParent<Boss_HP_manager>();
        if (bossHp != null)
        {
            bossHp.TakeDamage(damage, player_num);
            return;
        }
        if (currentHP <= 0)
        {
            DestroyEnemy(player_num);
        }
        else
        {
            // HPがまだ残っている場合、ダメージ効果音を鳴らす
            if (damageSound != null)
            {
                Debug.Log("Play damageSound!");
                AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position, 1.0f);
            }
        }
    }

    // 突進挙動コルーチン
    private IEnumerator ChargeRoutine()
    {
        while (true)
        {
            // 待機（min〜max の範囲から乱数）
            float waitTime = UnityEngine.Random.Range(waitBeforeCharge_min, waitBeforeCharge_max);
            yield return new WaitForSeconds(waitTime);

            // 突進前の座標を保存
            originalPosition = transform.position;

            // カメラを揺らす
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
            }

            // 突進の準備フェーズ: この間にダメージを受けると突進がキャンセルされる
            isPreparing = true;
            chargeCanceled = false;
            float t = 0f;
            while (t < prepareDuration)
            {
                if (chargeCanceled) break;
                t += Time.deltaTime;
                yield return null;
            }

            // 準備フラグを下げる
            isPreparing = false;
            if (chargeCanceled)
            {
                // キャンセルされた場合、必要ならエフェクトやリアクションを入れてから次のループへ
                Debug.Log("Charge cancelled by damage.");
                yield return new WaitForSeconds(0.1f);
                continue; // 次の待機へ戻る
            }

            // 準備が完了したので、突進に入る前に他スクリプトを無効化する
            foreach (var script in otherScripts)
            {
                script.enabled = false;
            }

            // スコアを減らす
            if (ScoreManager.instance != null)
            {
                // 全員のスコアを減らす
                for (int i = 0; i < ScoreManager.scores.Length; i++)
                {
                    ScoreManager.instance.DecreaseScore(i, scorePenaltyOnCharge);
                }
            }

            // 前方に頭突き
            Vector3 targetPos = transform.position + transform.forward * chargeDistance;
            isCharging = true;
            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, chargeSpeed * Time.deltaTime);
                yield return null;
            }

            // 少し待機（到着演出用、必要なら調整）
            yield return new WaitForSeconds(0.5f);

            // 元の座標へ戻る
            while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, returnSpeed * Time.deltaTime);
                yield return null;
            }
            isCharging = false;

            // 他のスクリプトを再度有効化
            foreach (var script in otherScripts)
            {
                script.enabled = true;
            }
        }
    }

    public void DestroyEnemy(int player_num)
    {
        if (!Application.isPlaying) return;

        OnDeath?.Invoke(); // 死亡イベントを呼び出す

        if (explodeSound != null)
        {
            Debug.Log("Play explodeSound!");
            AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, 1.0f);
        }

        if (breakEffect != null)
        {
            GameObject effect = Instantiate(breakEffect, transform.position + breakEffectOffset, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        if (ScoreManager.instance != null)
        {
            Debug.Log(player_num);
            ScoreManager.instance.AddScore(player_num, scoreValue);
        }

        Destroy(gameObject);
    }

    // 突進をキャンセルする（準備中にダメージを受けたときなど）
    private void CancelCharge()
    {
        if (!isPreparing) return;
        chargeCanceled = true;
        isPreparing = false;
        // キャンセル時のエフェクトを再生
        if (cancelEffectPrefab != null)
        {
            GameObject effect = Instantiate(cancelEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1.0f); // 1秒後に自動破棄
        }
        // キャンセル時の音を再生
        if (cancelSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(cancelSound, Camera.main.transform.position, 1.0f);
        }
        Debug.Log("CancelCharge called: charge will be cancelled.");
    }
}