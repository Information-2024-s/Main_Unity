using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioClip explodeSound; // 爆発音
    public GameObject breakEffect;
    public int scoreValue = 10;

    public void DestroyEnemy(int player_num)
    {
        // ゲーム中にのみ処理（エディタ停止時などを除外）
        if (!Application.isPlaying) return;

        // 音を鳴らす
        if (explodeSound != null)
        {
            Debug.Log("Play explodeSound!");
            AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, 1.0f);
        }

        // エフェクトを出す
        if (breakEffect != null)
        {
            GameObject effect = Instantiate(breakEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1.0f); // 自動削除
        }

        // スコア加算
        if (ScoreManager.instance != null)
        {
            Debug.Log(player_num);
            ScoreManager.instance.AddScore(player_num, scoreValue);
            
        }

        // 自身を削除
        Destroy(gameObject);
    }
}
