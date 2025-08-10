using UnityEngine;

public class MoveSideToSide : MonoBehaviour
{
    public float speed = 2.0f; // 移動速度
    public Vector3 distance = new Vector3(3.0f ,0.0f , 0.0f); // 左右前後上下の移動距離
    private Vector3 startPosition;
    private bool movingForward = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 移動方向を決定
        float targetX = movingForward ? startPosition.x + distance.x : startPosition.x - distance.x;
        float targetY = movingForward ? startPosition.y + distance.y : startPosition.y - distance.y;
        float targetZ = movingForward ? startPosition.z + distance.z : startPosition.z - distance.z;
        
        // 位置を補間
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, targetY, targetZ), speed * Time.deltaTime);
        // 目的地に到達したら反転
        if (Mathf.Abs(transform.position.x - targetX) +  Mathf.Abs(transform.position.y - targetY) + Mathf.Abs(transform.position.z - targetZ)< 0.1f)
        {
            movingForward = !movingForward;
        }
    }
}