using UnityEngine;

public class Enemy_circle : MonoBehaviour
{
    public Transform center;   // 円の中心
    public float radius = 3f;  // 半径
    public float speed = 2f;   // 角速度（速さ）

    private float angle = 0f;  // 現在の角度

    void Update()
    {
        // 時間に応じて角度を増加
        angle += speed * Time.deltaTime;

        // 新しい位置を計算
        float y = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // 中心の位置 + 円周上の座標
        transform.position = center.position + new Vector3(0, y, z);
    }
}
