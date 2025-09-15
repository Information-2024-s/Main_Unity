using UnityEngine;
using UnityEngine.Events; // ★追加

public class EnemyController : MonoBehaviour
{
    // --- 状態定義 ---
    private enum State
    {
        InitialMove, // 初期移動
        MainBehavior // 本来の動き
    }
    private State currentState;

    // --- 変数宣言 ---
    // これらがクラスのどこからでも使える変数です。
    // 名前と大文字・小文字が完全に一致している必要があります。
    public EnemyMovementType movementType;
    public float moveSpeed;
    
    // ジグザグ移動用の設定
    public float zigzagAmplitude = 2f;
    public float zigzagFrequency = 5f;

    // LastBoss1用の設定
    public float rectangleWidth = 5f;
    public float rectangleHeight = 5f;
    public float sideDuration = 2f;
    public BossInitialDirection initialDirection = BossInitialDirection.Right;

    // InitialMoveThenHoming用の設定
    public Vector3 initialMoveDirection;
    public float initialMoveDuration;
    public float waitDuration;

    // Circle運動用の設定
    public float circleRadius;
    public Vector3 circleCenterOffset;

    // LastBoss1の内部状態管理用
    private int currentSide = 0;
    private float sideTimer = 0f;
    private Vector3 startPosition;
    private Vector3 sideStartPosition;

    // InitialMoveThenHomingの状態管理用
    private enum InitialHomingState
    {
        InitialMove,
        Waiting,
        Homing
    }
    private InitialHomingState homingState;
    private float stateTimer;

    // Circle運動の内部状態管理用
    private Vector3 circleCenter;
    private float angle;

    // 初期移動用の変数
    private bool useMovePosition;
    private Vector3 targetMovePosition;
    private UnityEvent onInitialMoveComplete;


    // この変数は内部でのみ使うのでprivateのまま
    private Transform playerTransform;

    /// <summary>
    /// スポナーから呼び出される初期化メソッド
    /// </summary>
    public void Setup(EnemySpawnInfo spawnInfo)
    {
        // ここでクラス変数に値を設定しています
        movementType = spawnInfo.movementType;
        moveSpeed = spawnInfo.moveSpeed;
        zigzagAmplitude = spawnInfo.zigzagAmplitude;
        zigzagFrequency = spawnInfo.zigzagFrequency;
        rectangleWidth = spawnInfo.rectangleWidth;
        rectangleHeight = spawnInfo.rectangleHeight;
        sideDuration = spawnInfo.sideDuration;
        initialDirection = spawnInfo.initialDirection; // 開始方向を設定
        initialMoveDirection = spawnInfo.initialMoveDirection;
        initialMoveDuration = spawnInfo.initialMoveDuration;
        waitDuration = spawnInfo.waitDuration;
        circleRadius = spawnInfo.circleRadius;
        circleCenterOffset = spawnInfo.circleCenterOffset;

        // ★追加：初期移動の設定
        useMovePosition = spawnInfo.useMovePosition;
        targetMovePosition = spawnInfo.movePosition;
        onInitialMoveComplete = spawnInfo.onInitialMoveComplete; // イベントをコピー

        if (useMovePosition)
        {
            currentState = State.InitialMove;
        }
        else
        {
            currentState = State.MainBehavior;
            // 初期移動がない場合は、ここで各種移動パターンの初期化を行う
            InitializeMainBehavior();
        }
    }

    // 各移動パターンの初期化処理をまとめる
    private void InitializeMainBehavior()
    {
        // Homing系の移動でPlayerを探す
        if (movementType == EnemyMovementType.Homing || movementType == EnemyMovementType.InitialMoveThenHoming)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("追尾対象のプレイヤーが見つかりません。Playerタグを確認してください。");
                if (movementType == EnemyMovementType.Homing)
                {
                    movementType = EnemyMovementType.Straight; 
                }
            }
        }
        
        // 各移動パターンの初期位置やタイマーなどを設定
        switch(movementType)
        {
            case EnemyMovementType.LastBoss1:
                startPosition = transform.position;
                sideStartPosition = transform.position;
                sideTimer = 0f;
                currentSide = (int)initialDirection;
                break;
            case EnemyMovementType.InitialMoveThenHoming:
                homingState = InitialHomingState.InitialMove;
                stateTimer = 0f;
                break;
            case EnemyMovementType.Circle:
                startPosition = transform.position;
                circleCenter = startPosition + circleCenterOffset;
                angle = 0f;
                break;
        }
    }
    /// <summary>
    /// 毎フレーム呼ばれる更新処理
    /// </summary>
    void Update()
    {
        if (currentState == State.InitialMove)
        {
            MoveToTargetPosition();
        }
        else // currentState == State.MainBehavior
        {
            ExecuteMainBehavior();
        }
    }

    private void MoveToTargetPosition()
    {
        // targetMovePositionへの移動処理
        transform.position = Vector3.MoveTowards(transform.position, targetMovePosition, moveSpeed * Time.deltaTime);

        // 目的地に到着したら、状態をMainBehaviorに切り替え、イベントを実行
        if (Vector3.Distance(transform.position, targetMovePosition) < 0.01f)
        {
            currentState = State.MainBehavior;

            // ★追加：イベントを実行
            onInitialMoveComplete?.Invoke();

            InitializeMainBehavior();
        }
    }

    private void ExecuteMainBehavior()
    {
        switch (movementType)
        {
            case EnemyMovementType.None:
                // 何もしない
                break;
            case EnemyMovementType.Straight:
                MoveStraight();
                break;
            case EnemyMovementType.Zigzag:
                MoveZigzag();
                break;
            case EnemyMovementType.Homing:
                MoveHoming();
                break;
            case EnemyMovementType.LastBoss1:
                MoveLastBoss1();
                break;
            case EnemyMovementType.InitialMoveThenHoming:
                MoveInitialThenHoming();
                break;
            case EnemyMovementType.Circle:
                MoveCircle();
                break;
        }
    }

    private void MoveStraight()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void MoveZigzag()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        float zigzagAmount = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
        transform.Translate(Vector3.right * zigzagAmount * Time.deltaTime);
    }

    private void MoveHoming()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void MoveLastBoss1()
    {
        sideTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(sideTimer / sideDuration);

        Vector3 targetOffset = Vector3.zero;
        Vector3 currentDirectionVector = Vector3.zero;

        // 0:Right, 1:Up, 2:Left, 3:Down
        switch (currentSide % 4)
        {
            case 0: // 右へ
                currentDirectionVector = Vector3.right * rectangleWidth;
                break;
            case 1: // 上へ
                currentDirectionVector = Vector3.up * rectangleHeight;
                break;
            case 2: // 左へ
                currentDirectionVector = Vector3.left * rectangleWidth;
                break;
            case 3: // 下へ
                currentDirectionVector = Vector3.down * rectangleHeight;
                break;
        }

        // Lerpの開始点をsideStartPositionにすることで、各辺が前の辺の終点から正しく開始される
        transform.position = Vector3.Lerp(sideStartPosition, sideStartPosition + currentDirectionVector, progress);

        if (sideTimer >= sideDuration)
        {
            sideTimer = 0f;
            sideStartPosition = transform.position; // 次の辺の開始位置を現在の位置に更新
            currentSide = (currentSide + 1) % 4;

            // 1周して最初の方向に戻った時、位置を補正してループのズレを防ぐ
            if (currentSide == (int)initialDirection)
            {
                transform.position = startPosition;
                sideStartPosition = startPosition;
            }
        }
    }

    private void MoveInitialThenHoming()
    {
        stateTimer += Time.deltaTime;

        switch (homingState)
        {
            case InitialHomingState.InitialMove:
                // 指定された方向に移動
                transform.Translate(initialMoveDirection.normalized * moveSpeed * Time.deltaTime);
                if (stateTimer >= initialMoveDuration)
                {
                    // 待機状態に移行
                    homingState = InitialHomingState.Waiting;
                    stateTimer = 0f;
                }
                break;

            case InitialHomingState.Waiting:
                // 指定時間待機
                if (stateTimer >= waitDuration)
                {
                    // 追尾状態に移行
                    homingState = InitialHomingState.Homing;
                    stateTimer = 0f;
                    // プレイヤーが見つからない場合は直進する
                    if (playerTransform == null)
                    {
                        movementType = EnemyMovementType.Straight;
                    }
                }
                break;

            case InitialHomingState.Homing:
                // プレイヤーを追尾
                MoveHoming();
                break;
        }
    }

    private void MoveCircle()
    {
        angle += moveSpeed * Time.deltaTime;
        float x = Mathf.Cos(angle) * circleRadius;
        float z = Mathf.Sin(angle) * circleRadius;

        // Y軸は円運動に含めず、X-Z平面で動かす場合
        transform.position = circleCenter + new Vector3(x, 0, z);
    }
}