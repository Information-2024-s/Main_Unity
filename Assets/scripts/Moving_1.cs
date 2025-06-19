using UnityEngine;

public class MoveSideToSide : MonoBehaviour
{
    public float speed = 2.0f; // 移動速度
    public float distance = 3.0f; // 左右の移動距離
    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 移動方向を決定
        float targetX = movingRight ? startPosition.x + distance : startPosition.x - distance;
        
        // 位置を補間
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), speed * Time.deltaTime);

        // 目的地に到達したら反転
        if (Mathf.Abs(transform.position.x - targetX) < 0.1f)
        {
            movingRight = !movingRight;
        }
    }
}