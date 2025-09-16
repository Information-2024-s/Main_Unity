using UnityEngine;

public class Enemy_circle : MonoBehaviour
{
    // 中心座標をVector3で直接指定
    public Vector3 center = Vector3.zero; // 初期値を(0, 0, 0)に設定
    public float radius = 3f;             // 半径
    public float speed = 2f;              // 角速度（速さ）

    private float angle = 0f;             // 現在の角度

    void Update()
    {
        // 時間に応じて角度を増加
        angle += speed * Time.deltaTime;

        // 新しい位置を計算
        float y = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // 指定された中心座標 + 円周上の座標
        transform.position = center + new Vector3(0, y, z);
    }
}