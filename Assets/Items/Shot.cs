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
        // "monitor"タグを持つオブジェクトに当たった場合
        if (col.gameObject.CompareTag("monitor"))
        {
            // TargetObjectコンポーネントを取得
            TargetObject target = col.gameObject.GetComponent<TargetObject>();
            if (target != null)
            {
                Debug.Log("Monitor Hit!");
                // TargetObjectのTakeDamageを呼び出す
                target.TakeDamage(damage, player_num);
                Destroy(this.gameObject); // 弾を消す
                return; // 処理を終了
            }
        }

        // "Enemy"タグを持つオブジェクトに当たった場合
        if (col.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            BossEnemy bossEnemy = col.gameObject.GetComponent<BossEnemy>();
            Enemy_chuboss chuboss = col.gameObject.GetComponent<Enemy_chuboss>();
            if (enemy != null)
            {
                Debug.Log("Enemy Hit!");
                enemy.TakeDamage(damage, player_num);
            }
            else if (chuboss != null)
            {
                Debug.Log("Chuboss Hit!");
                chuboss.TakeDamage(damage, player_num);
            }
            else if (bossEnemy != null)
            {
                Debug.Log("Boss Enemy Hit!");
                bossEnemy.ApplyDamage(damage, player_num);
            }
            Destroy(this.gameObject); // 弾も削除
        }
    }
}