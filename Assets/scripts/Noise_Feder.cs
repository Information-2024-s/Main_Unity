using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// CanvasGroupコンポーネントを必須にする
[RequireComponent(typeof(CanvasGroup))]
public class NoiseFader : MonoBehaviour
{
    [Header("フェード設定")]
    [Tooltip("フェードイン/アウトにかかる時間（秒）")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("ノイズアニメーション設定")]
    [Tooltip("ノイズテクスチャをスクロールさせてアニメーションさせるか")]
    [SerializeField] private bool animateNoise = true;

    [Tooltip("ノイズのスクロール速度")]
    [SerializeField] private Vector2 noiseScrollSpeed = new Vector2(0.1f, 0.1f);

    // --- 内部参照 ---
    private CanvasGroup canvasGroup;
    private RawImage noiseImage; // テクスチャを動かすためRawImageを使用
    private Coroutine currentFadeCoroutine;
    private Coroutine noiseAnimationCoroutine;

    void Awake()
    {
        // 必要なコンポーネントを取得
        canvasGroup = GetComponent<CanvasGroup>();
        noiseImage = GetComponent<RawImage>();

        if (noiseImage == null)
        {
            Debug.LogError("このスクリプトにはRawImageコンポーネントが必要です。");
        }

        // 初期状態では透明にしておく
        canvasGroup.alpha = 255f;
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

        // ノイズアニメーションを開始
        if (animateNoise && noiseAnimationCoroutine == null)
        {
           noiseAnimationCoroutine = StartCoroutine(AnimateNoiseTexture());
        }
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

        // ノイズアニメーションを停止
        if (noiseAnimationCoroutine != null)
        {
            StopCoroutine(noiseAnimationCoroutine);
            noiseAnimationCoroutine = null;
        }
    }

    /// <summary>
    /// 指定されたアルファ値へ徐々に変化させるコルーチン
    /// </summary>
    /// <param name="targetAlpha">目標の透明度 (0.0f - 1.0f)</param>
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

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

    /// <summary>
    /// RawImageのUV Rectを動かしてノイズをアニメーションさせるコルーチン
    /// </summary>
    private IEnumerator AnimateNoiseTexture()
    {
        while (true)
        {
            if (noiseImage != null)
            {
                // UV座標を時間経過でずらす
                Rect currentUV = noiseImage.uvRect;
                currentUV.x += noiseScrollSpeed.x * Time.deltaTime;
                currentUV.y += noiseScrollSpeed.y * Time.deltaTime;
                noiseImage.uvRect = currentUV;
            }
            yield return null; // 1フレーム待機
        }
    }
}