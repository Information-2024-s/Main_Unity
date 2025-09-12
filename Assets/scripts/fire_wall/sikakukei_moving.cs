using UnityEngine;

/// <summary>
/// 最初に移動を開始する方向を定義します。
/// </summary>
public enum RectangleInitialDirection
{
    Right,
    Up,
    Left,
    Down
}

/// <summary>
/// GameObjectを四角形に沿って移動させる独立したコンポーネント。
/// このスクリプトをアタッチするだけで、そのオブジェクトが四角く動き始めます。
/// </summary>
public class SikakukeiMoving : MonoBehaviour
{
    [Header("四角形のサイズ")]
    [Tooltip("移動する四角形の幅")]
    public float rectangleWidth = 5f;
    [Tooltip("移動する四角形の高さ")]
    public float rectangleHeight = 5f;

    [Header("移動設定")]
    [Tooltip("四角形の一辺を移動するのにかかる時間（秒）")]
    public float sideDuration = 2f;
    [Tooltip("最初に移動を開始する方向")]
    public RectangleInitialDirection initialDirection = RectangleInitialDirection.Right;

    // --- 内部状態を管理する変数 ---
    private int currentSide;
    private float sideTimer;
    private Vector3 startPosition;
    private Vector3 sideStartPosition;

    /// <summary>
    /// コンポーネントが有効になった最初のフレームで呼び出されます。
    /// </summary>
    void Start()
    {
        // 開始時の位置（これが四角形の基準点になる）を記録
        startPosition = transform.position;
        sideStartPosition = transform.position;
        sideTimer = 0f;

        // インスペクターで設定された開始方向に応じて、最初の辺を決定
        currentSide = (int)initialDirection;
    }

    /// <summary>
    /// 毎フレーム呼び出されます。
    /// </summary>
    void Update()
    {
        // 四角形に移動する処理を呼び出し
        MoveInRectangle();
    }

    /// <summary>
    /// 四角形移動のメインロジック
    /// </summary>
    private void MoveInRectangle()
    {
        sideTimer += Time.deltaTime;
        // 現在の辺の移動の進行度を計算（0.0～1.0）
        float progress = Mathf.Clamp01(sideTimer / sideDuration);

        // 現在の辺の移動ベクトルを計算
        Vector3 targetOffset = GetTargetOffsetForCurrentSide();

        // 線形補間(Lerp)を使って、現在の辺の始点から終点へオブジェクトを滑らかに移動させる
        transform.position = Vector3.Lerp(sideStartPosition, sideStartPosition + targetOffset, progress);

        // 現在の辺の移動が完了したら
        if (progress >= 1.0f)
        {
            // タイマーをリセット
            sideTimer = 0f;
            // 次の辺の開始位置を、現在の辺の正確な終点に更新
            sideStartPosition += targetOffset;
            // 次の辺へ進む (0->1->2->3->0 とループ)
            currentSide = (currentSide + 1) % 4;

            // 1周して最初の辺に戻った時
            if (currentSide == (int)initialDirection)
            {
                // 浮動小数点数の誤差が蓄積するのを防ぐため、位置を最初の基準点にリセット
                transform.position = startPosition;
                sideStartPosition = startPosition;
            }
        }
    }

    /// <summary>
    /// 現在の辺（currentSide）に基づいて、移動先のオフセットベクトルを返します。
    /// </summary>
    private Vector3 GetTargetOffsetForCurrentSide()
    {
        switch (currentSide)
        {
            case 0: return Vector3.right * rectangleWidth;  // 右へ
            case 1: return Vector3.up * rectangleHeight;    // 上へ
            case 2: return Vector3.left * rectangleWidth;   // 左へ
            case 3: return Vector3.down * rectangleHeight;  // 下へ
            default: return Vector3.zero; // 念のため
        }
    }
}