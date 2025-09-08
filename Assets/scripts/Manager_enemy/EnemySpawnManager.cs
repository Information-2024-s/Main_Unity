using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("敵の発生パターンを設定するリスト")]
    public List<EnemySpawnInfo> spawnWave;

    // ゲームが開始された時に一度だけ呼ばれる
    void Start()
    {
        // 敵を発生させるコルーチンを開始する
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    /// <summary>
    /// 設定リストに従って敵を順番に発生させるコルーチン
    /// </summary>
    private IEnumerator SpawnEnemiesCoroutine()
    {
        // spawnWaveリストに登録された全ての敵を順番に処理する
        foreach (var spawnInfo in spawnWave)
        {
            // 1. 指定された時間だけ待機する
            yield return new WaitForSeconds(spawnInfo.timeSincePreviousSpawn);

            // 2. 敵のプレハブが設定されているか確認する
            if (spawnInfo.enemyPrefab != null)
            {
                Quaternion spawnRotation = Quaternion.Euler(spawnInfo.spawnRotationEuler); // 追加
                Instantiate(spawnInfo.enemyPrefab, spawnInfo.spawnPosition, spawnRotation);
                Debug.Log($"{spawnInfo.enemyPrefab.name} を {spawnInfo.spawnPosition} に発生させました。");
            }
            else
            {
                Debug.LogWarning("発生させる敵のプレハブが設定されていません。");
            }
        }

        Debug.Log("全ての敵の発生が完了しました。");
    }
}