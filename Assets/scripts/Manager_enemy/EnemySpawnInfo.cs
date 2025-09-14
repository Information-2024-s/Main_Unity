using UnityEngine;
using UnityEngine.Events;

// 敵の移動パターンの種類を定義 (enum)
public enum EnemyMovementType
{
    None,       // 何もしない
    Straight,   // まっすぐ進む
    Zigzag,     // ジグザグに動く
    Homing,     // プレイヤーを追いかける
    LastBoss1,   // 四角形に動く
    InitialMoveThenHoming, // ★追加：初動後ホーミング
    Circle      // ★追加: 円運動
}

// LastBoss1の開始方向
public enum BossInitialDirection
{
    Right, // 右から
    Up,    // 上から
    Left,  // 左から
    Down   // 下から
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public float timeSincePreviousSpawn;
    public Vector3 spawnPosition;
    public Vector3 spawnRotationEuler;

    [Header("リスポーン設定")]
    public bool respawns;
    public float respawnTime = 5f;

    [Header("レア敵設定")]
    public GameObject rareEnemyPrefab;
    [Range(0, 100)]
    public float rareEnemyChance;
}