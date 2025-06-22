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
            Destroy(col.gameObject); // 敵を削除
            Destroy(this.gameObject); // 弾を削除
        }
    }
}
