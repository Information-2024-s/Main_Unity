using UnityEngine;

// RigidbodyとColliderがあることを前提とする
[RequireComponent(typeof(Collider))]
public class BossEnemy : MonoBehaviour
{
    private Boss_HP_manager boss_HP_manager;

    void Start()
    {
        // 起動時に親オブジェクトからHP管理コンポーネントを探して保持する
        boss_HP_manager = GetComponentInParent<Boss_HP_manager>();
        if (boss_HP_manager == null)
        {
            Debug.LogError("親オブジェクトに Boss_HP_manager が見つかりません！", this.gameObject);
        }
    }

    // ダメージを親に伝えるための公開メソッド
    public void ApplyDamage(int damage, int player_num)
    {
        if (boss_HP_manager != null)
        {
            boss_HP_manager.TakeDamage(damage, player_num);
        }
    }
}