using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("敵の発生パターンを設定するリスト")]
    public List<EnemySpawnInfo> spawnWave;

    void Start()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        foreach (var spawnInfo in spawnWave)
        {
            yield return new WaitForSeconds(spawnInfo.timeSincePreviousSpawn);

            if (spawnInfo.enemyPrefab != null)
            {
                Quaternion spawnRotation = Quaternion.Euler(spawnInfo.spawnRotationEuler);

                // 1. 敵を生成し、GameObjectとして保持する
                GameObject spawnedEnemy = Instantiate(spawnInfo.enemyPrefab, spawnInfo.spawnPosition, spawnRotation);

                // 2. 生成した敵から EnemyController を取得する
                EnemyController controller = spawnedEnemy.GetComponent<EnemyController>();

                // 3. EnemyController に移動情報を設定する
                if (controller != null)
                {
                    controller.Setup(spawnInfo);
                }
                else
                {
                    Debug.LogWarning($"{spawnedEnemy.name} に EnemyController がアタッチされていません！");
                }

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