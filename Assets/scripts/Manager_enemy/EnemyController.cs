using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    // --- パラメータ（Setupメソッド経由で設定）---
    private EnemyMovementType movementType;
    private float moveSpeed;
    // (各移動パターンの詳細パラメータ)
    private Vector3 moveDirection;
    private float zigzagAmplitude, zigzagFrequency;
    private Vector3 sideToSideDistance;
    private List<Vector3> waypoints;
    private float waypointWaitTime;
    private List<Vector3> patrolCoordinates;
    private float coordinateWaitTime;
    private float circleRadius;
    private Vector3 circleCenterOffset;
    private float floatingAmplitude, floatingSpeed;
    private Vector3 initialMoveDirection;
    private float initialMoveDuration, homingWaitDuration;

    // --- 内部状態管理用の変数 ---
    private Transform playerTransform;
    private Coroutine movementCoroutine; // 巡回などのコルーチンを管理

    // 汎用
    private Vector3 startPosition;

    // Circle
    private Vector3 circleCenter;
    private float angle;

    // Floating
    private Vector3 initialFloatingPosition;

    // InitialMoveThenHoming
    private enum InitialHomingState { InitialMove, Waiting, Homing }
    private InitialHomingState homingState;
    private float stateTimer;

    /// <summary>
    /// スポナーから呼び出され、この敵の振る舞いをすべて設定する
    /// </summary>
    public void Setup(EnemySpawnInfo info)
    {
        // --- EnemySpawnInfoからすべての設定を受け取る ---
        movementType = info.movementType;
        moveSpeed = info.moveSpeed;
        moveDirection = info.moveDirection;
        zigzagAmplitude = info.zigzagAmplitude;
        zigzagFrequency = info.zigzagFrequency;
        sideToSideDistance = info.sideToSideDistance;
        waypoints = info.waypoints;
        waypointWaitTime = info.waypointWaitTime;
        patrolCoordinates = info.patrolCoordinates;
        coordinateWaitTime = info.coordinateWaitTime;
        circleRadius = info.circleRadius;
        circleCenterOffset = info.circleCenterOffset;
        floatingAmplitude = info.floatingAmplitude;
        floatingSpeed = info.floatingSpeed;
        initialMoveDirection = info.initialMoveDirection;
        initialMoveDuration = info.initialMoveDuration;
        homingWaitDuration = info.homingWaitDuration;
        
        // --- 初期化処理 ---
        startPosition = transform.position;
        InitializeBehavior();
    }

    private void InitializeBehavior()
    {
        // 既存のコルーチンが動いていれば停止
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
        
        // プレイヤーを探す必要がある移動タイプの場合
        if (movementType == EnemyMovementType.Homing || movementType == EnemyMovementType.InitialMoveThenHoming)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            else Debug.LogWarning("追尾対象のPlayerが見つかりません。");
        }

        // 移動タイプごとの初期設定
        switch (movementType)
        {
            case EnemyMovementType.WaypointPatrol:
                if (waypoints != null && waypoints.Count > 0)
                    movementCoroutine = StartCoroutine(WaypointPatrolRoutine());
                break;
            case EnemyMovementType.RandomCoordinatePatrol:
                if (patrolCoordinates != null && patrolCoordinates.Count > 0)
                    movementCoroutine = StartCoroutine(RandomCoordinatePatrolRoutine());
                break;
            case EnemyMovementType.SideToSide:
                movementCoroutine = StartCoroutine(SideToSideRoutine());
                break;
            case EnemyMovementType.Zigzag:
                movementCoroutine = StartCoroutine(ZigzagRoutine());
                break;
            case EnemyMovementType.Circle:
                circleCenter = startPosition + circleCenterOffset;
                angle = 0f;
                break;
            case EnemyMovementType.Floating:
                initialFloatingPosition = transform.position;
                break;
            case EnemyMovementType.InitialMoveThenHoming:
                homingState = InitialHomingState.InitialMove;
                stateTimer = 0f;
                break;
        }
    }

    void Update()
    {
        // Updateで完結する動き（コルーチンを使わないもの）をここで実行
        ExecuteImmediateBehavior();
    }

    private void ExecuteImmediateBehavior()
    {
        switch (movementType)
        {
            case EnemyMovementType.Straight:
                transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
                break;
            case EnemyMovementType.Homing:
                if (playerTransform != null)
                {
                    Vector3 direction = (playerTransform.position - transform.position).normalized;
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(direction);
                }
                break;
            case EnemyMovementType.Circle:
                angle += moveSpeed * Time.deltaTime;
                float x = Mathf.Cos(angle) * circleRadius;
                float z = Mathf.Sin(angle) * circleRadius;
                transform.position = circleCenter + new Vector3(x, 0, z);
                break;
            case EnemyMovementType.Floating:
                float yOffset = Mathf.Sin(Time.time * floatingSpeed) * floatingAmplitude;
                transform.position = new Vector3(initialFloatingPosition.x, initialFloatingPosition.y + yOffset, initialFloatingPosition.z);
                break;
            case EnemyMovementType.InitialMoveThenHoming:
                MoveInitialThenHoming();
                break;
        }
    }

    // --- コルーチンベースの移動処理 ---

    private IEnumerator WaypointPatrolRoutine()
    {
        int currentIndex = 0;
        while (true)
        {
            Vector3 targetPosition = waypoints[currentIndex];
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPosition;
            Debug.Log("目的地に到着。 " + waypointWaitTime + "秒待機します。");
            yield return new WaitForSeconds(waypointWaitTime);
            currentIndex = (currentIndex + 1) % waypoints.Count;
        }
    }

    private IEnumerator RandomCoordinatePatrolRoutine()
    {
        int lastIndex = -1;
        while (true)
        {
            int newIndex;
            do { newIndex = Random.Range(0, patrolCoordinates.Count); }
            while (patrolCoordinates.Count > 1 && newIndex == lastIndex);
            
            Vector3 targetPosition = patrolCoordinates[newIndex];
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(coordinateWaitTime);
            lastIndex = newIndex;
        }
    }

    private IEnumerator SideToSideRoutine()
    {
        Vector3 currentBasePosition = transform.position;
        bool movingToB = true;

        while (true)
        {
            Vector3 target = movingToB ? currentBasePosition + sideToSideDistance : currentBasePosition;
            
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = target; // 正確に位置を合わせる
            
            movingToB = !movingToB; // 方向転換
        }
    }

    private IEnumerator ZigzagRoutine()
    {
        Vector3 currentBasePosition = transform.position;
        float direction = 1f; 

        while (true)
        {
            // 前方に一定距離(moveDirection)進みつつ、横方向にジグザグ(zigzagAmplitude)するイメージ
            // moveDirectionを1セグメントの進行ベクトルとして使う
            Vector3 forwardMovement = moveDirection.normalized * moveSpeed; // 1秒あたりの進行ベクトル
            Vector3 sideMovement = Vector3.Cross(moveDirection, Vector3.up).normalized * zigzagAmplitude * direction;

            Vector3 targetPosition = currentBasePosition + forwardMovement + sideMovement;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPosition;

            // 次の基準点を更新
            currentBasePosition = targetPosition;
            // 横移動の方向を反転
            direction *= -1f;

            // 頻度(Frequency)を待機時間として解釈する
            if (zigzagFrequency > 0)
            {
                yield return new WaitForSeconds(1f / zigzagFrequency);
            }
        }
    }
    
    // --- Update内で呼ばれる複雑なロジック ---
    private void MoveInitialThenHoming()
    {
        stateTimer += Time.deltaTime;
        switch (homingState)
        {
            case InitialHomingState.InitialMove:
                transform.Translate(initialMoveDirection.normalized * moveSpeed * Time.deltaTime);
                if (stateTimer >= initialMoveDuration)
                {
                    homingState = InitialHomingState.Waiting;
                    stateTimer = 0f;
                }
                break;
            case InitialHomingState.Waiting:
                if (stateTimer >= homingWaitDuration)
                {
                    homingState = InitialHomingState.Homing;
                }
                break;
            case InitialHomingState.Homing:
                // Homingのロジックを再利用
                if (playerTransform != null)
                {
                    Vector3 direction = (playerTransform.position - transform.position).normalized;
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(direction);
                }
                break;
        }
    }
}
