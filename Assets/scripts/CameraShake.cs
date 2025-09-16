// ファイルパス: c:\Users\gamet\2Iクラス展\Assets\scripts\CameraShake.cs
using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // シングルトンインスタンス
    public static CameraShake Instance { get; private set; }

    private Coroutine shakeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// カメラを揺らす処理を呼び出す公開メソッド
    /// </summary>
    /// <param name="duration">揺れの継続時間（秒）</param>
    /// <param name="magnitude">揺れの強さ</param>
    public void Shake(float duration, float magnitude)
    {
        // 既に揺れている場合は、新しい揺れで上書きする
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    /// <summary>
    /// 実際にカメラを揺らすコルーチン
    /// </summary>
    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        Vector3 lastOffset = Vector3.zero;

        // 指定された時間、揺れ続ける
        while (elapsed < duration)
        {
            // 前のフレームのオフセットを元に戻す
            transform.localPosition -= lastOffset;

            // 新しいランダムなオフセットを生成
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Vector3 newOffset = new Vector3(x, y, 0);

            // カメラの位置に新しいオフセットを加える
            transform.localPosition += newOffset;
            lastOffset = newOffset;

            elapsed += Time.deltaTime;

            yield return null;
        }

        // 揺れが終わったら最後のオフセットを元に戻す
        transform.localPosition -= lastOffset;
        shakeCoroutine = null;
    }
}