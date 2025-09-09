using UnityEngine;

// 敵の移動パターンの種類を定義 (enum)
public enum EnemyMovementType
{
    Straight,   // まっすぐ進む
    Zigzag,     // ジグザグに動く
    Homing      // プレイヤーを追いかける
}

[System.Serializable]
public class EnemySpawnInfo
{
    // [Header("基本設定")] // ヘッダーは必須ではないので、好みで付けてください
    public GameObject enemyPrefab;
    public float timeSincePreviousSpawn;
    public Vector3 spawnPosition;
    public Vector3 spawnRotationEuler;

    // [Header("移動設定")] // こちらも同様
    public EnemyMovementType movementType; // どのパターンで動くか
    public float moveSpeed = 5f;           // 移動の速さ

    public float zigzagAmplitude = 2f;
    public float zigzagFrequency = 5f;
}