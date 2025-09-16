using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveInfo
    {
        public string waveName;
        public List<EnemySpawnInfo> enemies;
        public float timeToNextWave; // このWaveが開始してから次のWaveが始まるまでの時間
    }

    [Header("Waveごとの敵情報リスト")]
    public List<WaveInfo> waves;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<Coroutine> activeRespawnCoroutines = new List<Coroutine>();

    void Start()
    {
        StartCoroutine(HandleWavesCoroutine());
    }

    private IEnumerator HandleWavesCoroutine()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            // 実行中のリスポーン処理をすべて停止
            foreach (var coroutine in activeRespawnCoroutines)
            {
                StopCoroutine(coroutine);
            }
            activeRespawnCoroutines.Clear();

            // 前のWaveの敵が残っていれば全て削除
            foreach (var enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            spawnedEnemies.Clear();

            Debug.Log($"Wave {i + 1} 開始: {waves[i].waveName}");
            
            // Waveの敵出現コルーチンを非同期で開始
            StartCoroutine(SpawnWaveEnemies(waves[i]));

            // 次のWaveまでの待機時間
            yield return new WaitForSeconds(waves[i].timeToNextWave);
            
            Debug.Log($"Wave {i + 1} 終了");
        }
        Debug.Log("全てのWaveが終了しました。");
    }

    private IEnumerator SpawnWaveEnemies(WaveInfo wave)
    {
        float waveStartTime = Time.time;
        int spawnedCount = 0;

        while (spawnedCount < wave.enemies.Count)
        {
            float elapsedTime = Time.time - waveStartTime;
            var spawnInfo = wave.enemies[spawnedCount];

            if (elapsedTime >= spawnInfo.timeSincePreviousSpawn)
            {
                SpawnEnemy(spawnInfo);
                spawnedCount++;
            }
            yield return null;
        }
    }

    void SpawnEnemy(EnemySpawnInfo spawnInfo)
    {
        GameObject prefabToSpawn = spawnInfo.enemyPrefab;

        // レア敵の出現判定
        if (spawnInfo.rareEnemyPrefab != null && Random.Range(0f, 100f) < spawnInfo.rareEnemyChance)
        {
            prefabToSpawn = spawnInfo.rareEnemyPrefab;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("発生させる敵のプレハブが設定されていません。");
            return;
        }

        // 敵がカメラの方を向くようにY軸を180度回転させる
        Vector3 correctedRotationEuler = spawnInfo.spawnRotationEuler + new Vector3(0, 180, 0);
        Quaternion spawnRotation = Quaternion.Euler(correctedRotationEuler);
        GameObject spawnedEnemyObject = Instantiate(prefabToSpawn, spawnInfo.spawnPosition, spawnRotation);
        spawnedEnemies.Add(spawnedEnemyObject); // Wave切り替え時の削除用にリストに追加

        // ★ここを追加
        EnemyController controller = spawnedEnemyObject.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.Setup(spawnInfo);
        }
        else
        {
            Debug.LogWarning($"プレハブ '{spawnedEnemyObject.name}' にEnemyControllerスクリプトがアタッチされていません。移動設定が適用されません。");
        }

        Enemy enemyComponent = spawnedEnemyObject.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            // ラムダ式を使って、どのspawnInfoで死んだかを渡す
            enemyComponent.OnDeath += () => OnEnemyDied(spawnInfo, spawnedEnemyObject);
        }
        
        Debug.Log($"{prefabToSpawn.name} を {spawnInfo.spawnPosition} に発生させました。");
    }

    void OnEnemyDied(EnemySpawnInfo spawnInfo, GameObject deadEnemy)
    {
        // Wave切り替え時に削除されるリストからも削除
        spawnedEnemies.Remove(deadEnemy);

        if (spawnInfo.respawns)
        {
            Coroutine respawnCoroutine = null;
            respawnCoroutine = StartCoroutine(RespawnEnemyCoroutine(spawnInfo, co => activeRespawnCoroutines.Remove(respawnCoroutine)));
            activeRespawnCoroutines.Add(respawnCoroutine);
        }
    }

    IEnumerator RespawnEnemyCoroutine(EnemySpawnInfo spawnInfo, System.Action<Coroutine> onComplete)
    {
        yield return new WaitForSeconds(spawnInfo.respawnTime);
        Debug.Log($"リスポーン: {spawnInfo.enemyPrefab.name}");
        SpawnEnemy(spawnInfo);
        onComplete(null); // 完了を通知
    }
}