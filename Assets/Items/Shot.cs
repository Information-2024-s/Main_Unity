using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private float deleteTime = 2f;

    void Start()
    {
        Destroy(this.gameObject, deleteTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().DestroyEnemy();
            Destroy(this.gameObject); // 弾も削除
        }
    }
}
