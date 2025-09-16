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
    public float waitBeforeCharge = 2.0f; // 突進前の待機秒数
    private float chargeDistance = -1.0f; // 突進する距離
    private float chargeSpeed = 30.0f; // 突進速度
    private float returnSpeed = 30.0f; // 戻る速度
    public int scorePenaltyOnCharge = 50; // 突進時に減らすスコア
    public float shakeDuration = 0.3f; // カメラを揺らす時間
    public float shakeMagnitude = 0.1f; // カメラを揺らす強さ

    public event Action OnDeath; // 死亡時に呼ばれるイベント

    private Vector3 originalPosition; // 元の座標
    private bool isCharging = false;
    private int currentHP;
    private MonoBehaviour[] otherScripts; // 自分以外のスクリプト

    void Start()
    {
        currentHP = maxHP;
        originalPosition = transform.position;

        // 自分以外の有効なMonoBehaviourを取得
        otherScripts = GetComponents<MonoBehaviour>().Where(script => script != this && script.enabled).ToArray();

        // 突進コルーチンを開始
        StartCoroutine(ChargeRoutine());
    }

    // ダメージを受ける
    public void TakeDamage(int damage, int player_num)
    {
        currentHP -= damage;
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
            // 待機
            yield return new WaitForSeconds(waitBeforeCharge);

            // 突進前の座標を保存
            originalPosition = transform.position;

            // カメラを揺らす
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
            }

            // 他のスクリプトを無効化
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
}