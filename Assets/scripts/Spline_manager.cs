using UnityEngine;
using UnityEngine.Splines;

public class Spline_manager : MonoBehaviour
{
    public SplineAnimate splineAnimate;

    void LateUpdate()
    {
        if (splineAnimate != null)
        {
            // Splineに沿った位置を取得
            transform.position = splineAnimate.transform.position;

            // 回転はY軸だけ反映（水平を維持）
            var rot = splineAnimate.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
        }
    }
}