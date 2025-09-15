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
    // [Header("基本設定")] // ヘッダーは必須ではないので、好みで付けてください
    public GameObject enemyPrefab;
    public float timeSincePreviousSpawn;
    public Vector3 spawnPosition;
    public bool useMovePosition; // ★追加：移動座標を使用するか
    public Vector3 movePosition;  // ★追加：移動先の座標
    public Vector3 spawnRotationEuler;
    public UnityEvent onInitialMoveComplete; // ★追加：初期移動完了時のイベント

    // [Header("移動設定")] // こちらも同様
    public EnemyMovementType movementType; // どのパターンで動くか
    public float moveSpeed = 5f;           // 移動の速さ

    public float zigzagAmplitude = 2f;
    public float zigzagFrequency = 5f;

    // LastBoss1用
    public float rectangleWidth = 5f;
    public float rectangleHeight = 5f;
    public float sideDuration = 2f;
    public BossInitialDirection initialDirection = BossInitialDirection.Right;

    // InitialMoveThenHoming用
    public Vector3 initialMoveDirection = Vector3.forward;
    public float initialMoveDuration = 2f;
    public float waitDuration = 1f;

    // Circle用
    public float circleRadius = 3f;
    public Vector3 circleCenterOffset = Vector3.zero;
}