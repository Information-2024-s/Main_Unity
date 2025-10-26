using UnityEngine;
using System.Collections;

// CanvasGroupコンポーネントを必須にする
[RequireComponent(typeof(CanvasGroup))]
public class NoiseFader : MonoBehaviour
{
    [Header("フェード設定")]
    [Tooltip("フェードイン/アウトにかかる時間（秒）")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Tooltip("初期アルファ (0.0 - 1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float initAlpha = 0f;

    // --- 内部参照 ---
    private CanvasGroup canvasGroup;
    private Coroutine currentFadeCoroutine;

    void Awake()
    {
        // 必要なコンポーネントを取得
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup コンポーネントが必要です。");
            return;
        }

        // 初期状態のアルファを設定
        canvasGroup.alpha = Mathf.Clamp01(initAlpha);
    }

    /// <summary>
    /// フェードインを開始する公開メソッド
    /// </summary>
    public void StartFadeIn()
    {
        // 既存のフェード処理があれば停止し、新しい処理を開始
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(Fade(1f));
    }

    /// <summary>
    /// フェードアウトを開始する公開メソッド
    /// </summary>
    public void StartFadeOut()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(Fade(0f));
    }

    /// <summary>
    /// 指定されたアルファ値へ徐々に変化させるコルーチン
    /// </summary>
    /// <param name="targetAlpha">目標の透明度 (0.0f - 1.0f)</param>
    private IEnumerator Fade(float targetAlpha)
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            currentFadeCoroutine = null;
            yield break;
        }

        while (time < fadeDuration)
        {
            // 経過時間割合を計算し、アルファ値を更新
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null; // 1フレーム待機
        }

        // 最終的なアルファ値を設定
        canvasGroup.alpha = targetAlpha;
        currentFadeCoroutine = null;
    }
}