using UnityEngine;
using System.Collections;

public class Boss_HP_manager : MonoBehaviour
{
    [Header("ボス本体の設定")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int scoreValue = 1000;

    [Header("破壊時のエフェクト")]
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private AudioClip explodeSound;
    [SerializeField] private Vector3 breakEffectOffset = Vector3.zero;

    private int currentHP;
    private bool isDestroyed = false; // 破壊処理の重複呼び出しを防ぐフラグ

    void Start()
    {
        int playerCount = Mathf.Max(1, player_manager.player_count);
        
        currentHP = maxHP * playerCount; // プレイヤー数に応じてHPを増加

        Debug.Log($"ボスHPが設定されました。プレイヤー数: {playerCount}, 最大HP: {currentHP}");
    }

    // ダメージを受ける処理（子オブジェクトから呼ばれる）
    public void TakeDamage(int damage, int player_num)
    {
        if (isDestroyed) return; // 既に破壊処理が始まっていれば何もしない

        currentHP -= damage;
        Debug.Log($"ボスがダメージを受けた！残りHP: {currentHP}");

        if (currentHP <= 0)
        {
            isDestroyed = true; // 破壊処理を開始
            StartCoroutine(DestroyBoss(player_num));
        }
    }

    private IEnumerator DestroyBoss(int player_num)
    {
        // Colliderを無効化して、これ以上ダメージを受けないようにする
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // ホワイトアウトを開始
        LastBoss.Instance?.FadeToWhite();

        // エフェクトとサウンドを再生
        if (breakEffect != null)
        {
            GameObject effect = Instantiate(breakEffect, transform.position + breakEffectOffset, Quaternion.identity);
            Destroy(effect, 1.0f);
        }
        if (explodeSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, 1.0f);
        }

        // スコアを加算
        ScoreManager.instance?.AddScore(player_num, scoreValue);

        // ボス本体（親オブジェクト）を破壊
        Destroy(gameObject);
        
        // ホワイトアウトの待機時間
        yield return new WaitForSeconds(2.0f);
    }

    // HPを外部から参照したい場合（例：UI表示）
    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;
}