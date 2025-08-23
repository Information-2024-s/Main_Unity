using UnityEngine;
using System.Collections;

public class EnemyZigZag : MonoBehaviour
{
    public float speed = 2.0f;                // 移動速度
    public Vector3 moveDistance = new Vector3(3.0f, 3.0f, 0.0f); // 移動する距離
    public float stopTime = 1.0f;             // 停止時間

    private bool isMoving = true;             // 移動中フラグ

    void Start()
    {
        // コルーチンを開始
        StartCoroutine(ZigZagMove());
    }

    IEnumerator ZigZagMove()
    {
        while (true) // 永遠に繰り返す
        {
            if (isMoving)
            {
                // 移動処理
                transform.position += moveDistance * speed * Time.deltaTime;

                // 移動が終わったら反転
                moveDistance.y *= -1;

                // 停止
                isMoving = false;
                yield return new WaitForSeconds(stopTime);

                // 再び動く
                isMoving = true;
            }
            yield return null;
        }
    }
}