using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    [Header("発生させる敵のプレハブ")]
    public GameObject enemyPrefab;

    [Header("前の敵が発生してからの待機時間 (秒)")]
    public float timeSincePreviousSpawn;

    [Header("発生させる座標")]
    public Vector3 spawnPosition;

    [Header("発生時の向き（Euler角）")]
    public Vector3 spawnRotationEuler = Vector3.zero; // 追加
}