using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public event Action OnDeath; // 死亡時に呼ばれるイベント
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
        currentHP -= damage;
        if (currentHP <= 0)
        {
            DestroyEnemy(player_num);
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