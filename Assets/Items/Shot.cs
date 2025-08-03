using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private float deleteTime = 2f;
    [SerializeField] private int player_num;

    void Start()
    {
        Destroy(this.gameObject, deleteTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().DestroyEnemy(player_num);
            Destroy(this.gameObject); // 弾も削除

        }
    }
}
