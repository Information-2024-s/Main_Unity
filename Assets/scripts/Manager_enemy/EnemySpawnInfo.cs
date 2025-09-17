using UnityEngine;
using System.Collections.Generic; // Listを使うために必要

// 敵の移動パターンの種類を定義 (enum)
// 提供されたスクリプトの機能をすべて網羅するように拡張
public enum EnemyMovementType
{
    None,                   // 何もしない
    Straight,               // 指定方向に直進
    Zigzag,                 // 前進しながら左右にジグザグ
    Homing,                 // プレイヤーを追尾
    SideToSide,             // 指定距離を往復
    WaypointPatrol,         // 指定した複数の地点を巡回
    RandomCoordinatePatrol, // 指定した座標リストからランダムに巡回
    Circle,                 // 中心点の周りを円運動
    Floating,               // 初期位置で上下に浮遊
    InitialMoveThenHoming   // 指定方向に初動後、プレイヤーを追尾
    // --- かつてのLastBoss1の動きはWaypointPatrolで代用可能です ---
}

[System.Serializable]
public class EnemySpawnInfo
{
    [Header("基本設定")]
    public string enemyName = "Enemy"; // インスペクターで識別しやすくするための名前
    public GameObject enemyPrefab;
    public float timeSincePreviousSpawn;
    public Vector3 spawnPosition;
    public Vector3 spawnRotationEuler;

    [Header("移動設定")]
    public EnemyMovementType movementType = EnemyMovementType.Straight;
    public float moveSpeed = 5f;

    // --- 各移動タイプごとの詳細設定 ---
    // インスペクターではEnemyControllerEditorによって必要なものだけ表示されます

    // Straight
    public Vector3 moveDirection = Vector3.forward;

    // Zigzag
    public float zigzagAmplitude = 1f;
    public float zigzagFrequency = 5f;

    // SideToSide
    public Vector3 sideToSideDistance = new Vector3(5, 0, 0);

    // WaypointPatrol
    public List<Vector3> waypoints; // ← List<Transform> から List<Vector3> に変更
    public float waypointWaitTime = 1f;

    // RandomCoordinatePatrol
    public List<Vector3> patrolCoordinates;
    public float coordinateWaitTime = 2f;

    // Circle
    public float circleRadius = 3f;
    public bool circleClockwise = false; // false:反時計回り, true:時計回り
    public Vector3 circleCenterOffset = Vector3.zero;
    [Space(10)]
    [Tooltip("円形に配置する敵の数。1より大きい場合、spawnPositionが中心点になります。")]
    public int circlePlacementCount = 1;
    [Tooltip("円形配置の半径")]
    public float circlePlacementRadius = 5f;

    // Floating
    public float floatingAmplitude = 0.5f;
    public float floatingSpeed = 1f;

    // InitialMoveThenHoming
    public Vector3 initialMoveDirection = Vector3.forward;
    public float initialMoveDuration = 2f;
    public float homingWaitDuration = 1f;


    [Header("リスポーン設定")]
    public bool respawns;
    public float respawnTime = 5f;

    [Header("レア敵設定")]
    public GameObject rareEnemyPrefab;
    [Range(0, 100)]
    public float rareEnemyChance;
}