using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCoordinatePatrol : MonoBehaviour
{
    // 移動地点の座標リスト
    public List<Vector3> patrolPoints;

    // 移動速度
    public float speed = 5.0f;

    // 各地点に到着してから、次の移動を開始するまでの待機時間（秒）
    public float interval = 3.0f;

    // 前回移動した地点のインデックスを記録
    private int lastIndex = -1;

    void Start()
    {
        // patrolPointsリストが設定されているか、また中身が空でないかを確認
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            StartCoroutine(PatrolRoutine());
        }
        else
        {
            Debug.LogError("移動地点の座標が設定されていません。");
        }
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // 次の移動先のインデックスをランダムに選ぶ
            int newIndex;

            if (patrolPoints.Count > 1)
            {
                do
                {
                    newIndex = Random.Range(0, patrolPoints.Count);
                } while (newIndex == lastIndex);
            }
            else
            {
                newIndex = 0;
            }

            // 次の目標座標をリストから取得
            Vector3 targetPosition = patrolPoints[newIndex];
            Debug.Log(targetPosition + " へ移動を開始します。");

            // --- 移動処理 ---
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
            
            // --- 移動完了 ---
            transform.position = targetPosition;
            Debug.Log(targetPosition + " に到着しました。");
            
            lastIndex = newIndex;

            yield return new WaitForSeconds(interval);
        }
    }
}