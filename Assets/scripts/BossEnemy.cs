using UnityEngine;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    public AudioClip explodeSound; // 爆発音
    public GameObject breakEffect;
    public int scoreValue = 10;

    public int maxHP = 10; // HP追加
    private int currentHP;

    public Vector3 breakEffectOffset = Vector3.zero; // オフセット追加

    void Start()
    {
        currentHP = maxHP;
    }

    // ダメージを受ける
    public void TakeDamage(int damage, int player_num)
    {
        if (currentHP <= 0) return;

        currentHP -= damage;
        if (currentHP <= 0)
        {
            StartCoroutine(DestroyEnemy(player_num));
        }
    }

    public IEnumerator DestroyEnemy(int player_num)
    {
        // まず、他のコンポーネントを無効にして、敵が動いたり攻撃したりしないようにする
        // 例: GetComponent<Collider>().enabled = false;
        // 例: GetComponent<Rigidbody>().isKinematic = true;

        // ホワイトアウトを開始
        LastBoss.Instance.FadeToWhite();

        // サウンドやエフェクトを再生
        if (explodeSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, 1.0f);
        }
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position + breakEffectOffset, Quaternion.identity);
        }

        // スコアを加算
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(player_num, scoreValue);
        }

        // last_bossに設定されているホワイトアウトの時間だけ待機する
        // ※この方法を使うには、last_bossのfadeToWhiteDurationをpublicにする必要があります。
        //   (今回は直接秒数を書く例を示します)
        yield return new WaitForSeconds(2.0f); // 例として2秒待つ

        // 待機後、自身を破壊する
        Destroy(gameObject);
    }
}