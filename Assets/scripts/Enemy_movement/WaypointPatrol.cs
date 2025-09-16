using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPatrol : MonoBehaviour
{
    // 巡回する地点のリスト (Vector3のリストに変更)
    public List<Vector3> patrolPoints;

    // オブジェクトの移動速度
    public float speed = 3.0f;

    // 各地点での待機時間（クールタイム）
    public float waitTime = 2.0f;

    // 次の目標地点のインデックス
    private int currentWaypointIndex = 0;

    void Start()
    {
        // patrolPointsリストが空でないことを確認 (変数名を変更)
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            // 巡回を開始するコルーチンを呼び出す
            StartCoroutine(Patrol());
        }
        else
        {
            Debug.LogError("巡回する座標が設定されていません。");
        }
    }

    IEnumerator Patrol()
    {
        // 無限ループで巡回を続ける
        while (true)
        {
            // 現在の目標座標を取得 (TransformからVector3に変更)
            Vector3 targetPosition = patrolPoints[currentWaypointIndex];

            // 目標地点に到着するまで移動を続ける (参照先を.positionから直接Vector3に変更)
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                // 現在位置から目標地点へ、指定した速度で移動
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                
                // 次のフレームまで待機
                yield return null; 
            }

            // 目標地点に到着したので、正確な位置に設定
            transform.position = targetPosition;

            // 指定した秒数だけ待機（クールタイム）
            Debug.Log("目的地に到着。 " + waitTime + "秒待機します。");
            yield return new WaitForSeconds(waitTime);

            // 次の目標地点のインデックスを更新
            // リストの最後まで行ったら、インデックスを0に戻す
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Count;
        }
    }
}