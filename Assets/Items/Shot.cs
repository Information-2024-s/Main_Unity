using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private float deleteTime = 2f;
    [SerializeField] private int player_num;
    [SerializeField] private int damage = 1; // ダメージ量追加

    void Start()
    {
        Destroy(this.gameObject, deleteTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            BossEnemy bossEnemy = col.gameObject.GetComponent<BossEnemy>();
            if (enemy != null)
            {
                Debug.Log("Enemy Hit!");
                enemy.TakeDamage(damage, player_num);
            }
            else if (bossEnemy != null)
            {
                Debug.Log("Boss Enemy Hit!");
                bossEnemy.TakeDamage(damage, player_num);
            }
            Destroy(this.gameObject); // 弾も削除
        }
    }
}