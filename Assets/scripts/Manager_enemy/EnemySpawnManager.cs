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
        // 再生モード中でなければ実行しない (エディター起因のエラー防止)
        if (!Application.isPlaying) return;

        // --- 観覧車（グループ回転）ロジック ---
        if (spawnInfo.movementType == EnemyMovementType.Circle && spawnInfo.circlePlacementCount > 1)
        {
            // 1. 観覧車の中心となるPivotオブジェクトを生成
            GameObject pivotObject = new GameObject($"FerrisWheel_{spawnInfo.enemyName}");
            pivotObject.transform.position = spawnInfo.spawnPosition;
            spawnedEnemies.Add(pivotObject); // PivotもWave終了時に破棄するリストに追加

            // 2. Pivotに回転を制御するコントローラーをアタッチして設定
            var groupController = pivotObject.AddComponent<CircleGroupController>();
            groupController.rotationSpeed = spawnInfo.moveSpeed;
            groupController.clockwise = spawnInfo.circleClockwise;

            // 3. 回転軸をカメラに対して最適化
            Vector3 camForward = Camera.main.transform.forward;
            float dotX = Mathf.Abs(Vector3.Dot(camForward, Vector3.right));
            float dotY = Mathf.Abs(Vector3.Dot(camForward, Vector3.up));
            float dotZ = Mathf.Abs(Vector3.Dot(camForward, Vector3.forward));

            if (dotX > dotY && dotX > dotZ) groupController.rotationAxis = Vector3.right;
            else if (dotY > dotZ) groupController.rotationAxis = Vector3.up;
            else groupController.rotationAxis = Vector3.forward;

            // 4. 敵を円周上に子オブジェクトとして配置
            for (int i = 0; i < spawnInfo.circlePlacementCount; i++)
            {
                float angle = i * (360f / spawnInfo.circlePlacementCount);
                float radian = angle * Mathf.Deg2Rad;
                
                // ★★★ 修正点：回転軸に合わせた平面に敵を配置する ★★★
                Vector3 localOffset;
                if (groupController.rotationAxis == Vector3.right) // YZ平面
                {
                    localOffset = new Vector3(0, Mathf.Cos(radian), Mathf.Sin(radian)) * spawnInfo.circlePlacementRadius;
                }
                else if (groupController.rotationAxis == Vector3.up) // XZ平面
                {
                    localOffset = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * spawnInfo.circlePlacementRadius;
                }
                else // XY平面
                {
                    localOffset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * spawnInfo.circlePlacementRadius;
                }

                Vector3 spawnPos = pivotObject.transform.position + localOffset;

                // --- 個々の敵の生成（単体生成とほぼ同じロジック） ---
                GameObject prefabToSpawn = spawnInfo.enemyPrefab;
                if (spawnInfo.rareEnemyPrefab != null && Random.Range(0f, 100f) < spawnInfo.rareEnemyChance) 
                    prefabToSpawn = spawnInfo.rareEnemyPrefab;
                
                if (prefabToSpawn == null) continue;

                GameObject enemyObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                enemyObj.transform.SetParent(pivotObject.transform); // Pivotの子にする
                spawnedEnemies.Add(enemyObj);

                // 敵の向きをPivotの中心方向から外側に向ける
                enemyObj.transform.up = (enemyObj.transform.position - pivotObject.transform.position).normalized;

                // 個々の敵は動かないように、MovementTypeをNoneにしてSetupを呼ぶ
                var noMoveInfo = spawnInfo;
                noMoveInfo.movementType = EnemyMovementType.None;
                EnemyController controller = enemyObj.GetComponent<EnemyController>();
                if (controller != null) controller.Setup(noMoveInfo);

                Enemy enemyComponent = enemyObj.GetComponent<Enemy>();
                if (enemyComponent != null) enemyComponent.OnDeath += () => OnEnemyDied(spawnInfo, enemyObj);
            }
        }
        else
        {
            // --- 通常の単体生成ロジック ---
            GameObject prefabToSpawn = spawnInfo.enemyPrefab;
            if (spawnInfo.rareEnemyPrefab != null && Random.Range(0f, 100f) < spawnInfo.rareEnemyChance)
                prefabToSpawn = spawnInfo.rareEnemyPrefab;

            if (prefabToSpawn == null)
            {
                Debug.LogWarning("発生させる敵のプレハブが設定されていません。");
                return;
            }

            Vector3 directionFromCamera = spawnInfo.spawnPosition - Camera.main.transform.position;
            directionFromCamera.y = 0;
            Quaternion spawnRotation = Quaternion.LookRotation(directionFromCamera);

            GameObject spawnedEnemyObject = Instantiate(prefabToSpawn, spawnInfo.spawnPosition, spawnRotation);
            spawnedEnemies.Add(spawnedEnemyObject);

            EnemyController controller = spawnedEnemyObject.GetComponent<EnemyController>();
            if (controller != null) controller.Setup(spawnInfo);
            else Debug.LogWarning($"プレハブ '{spawnedEnemyObject.name}' にEnemyControllerスクリプトがアタッチされていません。");

            Enemy enemyComponent = spawnedEnemyObject.GetComponent<Enemy>();
            if (enemyComponent != null) enemyComponent.OnDeath += () => OnEnemyDied(spawnInfo, spawnedEnemyObject);
            
            Debug.Log($"{prefabToSpawn.name} を {spawnInfo.spawnPosition} に発生させました。");
        }
    }

    void OnEnemyDied(EnemySpawnInfo spawnInfo, GameObject deadEnemy)
    {
        spawnedEnemies.Remove(deadEnemy);
        if (spawnInfo.respawns)
        {
            // 注意：観覧車の一部が死んだ場合、観覧車全体がリスポーンします
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
