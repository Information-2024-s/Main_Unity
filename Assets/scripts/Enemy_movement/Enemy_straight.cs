using UnityEngine;

public class Enemy_straight : MonoBehaviour
{
    public float speed = 3f; // m/s
    [SerializeField] public Vector3 moveDirection = Vector3.forward; // 移動方向をInspectorで設定

    void Update()
    {
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime);
    }
}