using UnityEngine;

/// <summary>
/// オブジェクトを初期位置を基準に上下に動かし、浮遊しているような効果を生み出します。
/// </summary>
public class Floating : MonoBehaviour
{
    [Header("浮遊設定")]
    [SerializeField, Tooltip("上下に動く速さ")]
    private float speed = 1.0f;

    [SerializeField, Tooltip("上下に動く幅の大きさ")]
    private float amplitude = 0.2f;

    // オブジェクトの初期位置を保存する変数
    private Vector3 initialPosition;

    void Start()
    {
        // スクリプトが開始された時点での位置を初期位置として保存
        initialPosition = transform.position;
    }

    void Update()
    {
        // Sin関数を使って、-1から1の範囲で滑らかに変化する値を計算
        // Time.time * speed で時間の経過とともに値が変化する速さを調整
        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;

        // 新しいY座標を「初期位置 + 計算したオフセット」で求める
        float newY = initialPosition.y + yOffset;

        // オブジェクトの位置を更新
        // XとZは初期位置のまま、Y座標だけを変更することで上下運動を実現
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}