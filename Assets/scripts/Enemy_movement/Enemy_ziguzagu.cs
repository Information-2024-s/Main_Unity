using UnityEngine;
using System.Collections;

public class EnemyZigZag : MonoBehaviour
{
    public float speed = 2.0f;                 // 移動速度
    public Vector2 moveDistance = new Vector2(3.0f, 3.0f); // 移動する距離（片道）
    public float stopTime = 1.0f;              // 停止時間

    private Vector3 startPos;                  // 基準位置
    private int directionX = 1;                // X方向の移動向き（1:右、-1:左）
    private int directionY = 1;                // Y方向の移動向き（1:上、-1:下）

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(ZigZagMove());
    }

    IEnumerator ZigZagMove()
    {
        while (true)
        {
            // 次のターゲット位置を計算（斜め方向）
            Vector3 targetPos = startPos + new Vector3(
                moveDistance.x * directionX,
                moveDistance.y * directionY,
                0
            );

            // ターゲット位置に向かって移動
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    speed * Time.deltaTime
                );
                yield return null;
            }

            // 停止時間
            yield return new WaitForSeconds(stopTime);

            // 方向反転（XとY両方を反転させる）
            //directionX *= -1;
            directionY *= -1;

            // 新しい基準位置を更新
            startPos = targetPos;
        }
    }
}
