using UnityEngine;

using System.Collections.Generic;

public class CircleGroupController : MonoBehaviour
{
    public PlaneType plane = PlaneType.XZ;
    public List<Transform> enemies = new List<Transform>();
    public float radius = 5f;
    public float rotationSpeed = 30f; // 度/秒
    public bool clockwise = true;
    private float currentAngle = 0f;
        public List<Quaternion> enemyRotations = new List<Quaternion>(); // 各敵の回転値

    void Update()
    {
        float direction = clockwise ? -1f : 1f;
        currentAngle += rotationSpeed * direction * Time.deltaTime;

        int count = Mathf.Max(enemies.Count, 1);
        float angleStep = 360f / count;

        // どの平面で配置するかを一度だけログ出力
        #if UNITY_EDITOR
        if (!logOnce)
        {
            Debug.Log($"[CircleGroupController] 配置平面: {plane}");
            logOnce = true;
        }
        #endif
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null) continue;
            float angle = currentAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 localPos;
            switch (plane)
            {
                case PlaneType.XZ:
                    localPos = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
                    break;
                case PlaneType.YZ:
                    localPos = new Vector3(0, Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
                    break;
                case PlaneType.XY:
                    localPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
                    break;
                default:
                    localPos = Vector3.zero;
                    break;
            }
            enemies[i].position = transform.position + localPos;
                // 指定した回転を常時適用
                if (i < enemyRotations.Count)
                    enemies[i].rotation = enemyRotations[i];
        }
    }

    // Editor再生中のみ一度だけ平面ログを出すためのフラグ
#if UNITY_EDITOR
    private bool logOnce = false;
#endif
}
