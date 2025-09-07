using UnityEngine;
using System.Collections;

public class LastBoss : MonoBehaviour
{
    // シングルトンインスタンス
    public static LastBoss Instance { get; private set; }

    /// <summary>
    /// 現在、画面効果（フェードなど）が実行中かどうか
    /// </summary>
    public bool IsFading { get; private set; } = false;

    [Header("参照")]
    [Tooltip("停止させたいGameTimerスクリプト")]
    [SerializeField] private GameTimer gameTimer;

    [Header("UI設定")]
    [Tooltip("画面フラッシュに使用するUIのCanvasGroup")]
    [SerializeField] private CanvasGroup flashCanvasGroup;

    // --- ここから追加 ---
    [Header("オーディオ設定")]
    [Tooltip("フェードアウトさせたいBGMのAudioSource")]
    [SerializeField] private AudioSource bgmAudioSource;
    // --- ここまで追加 ---

    [Header("フラッシュ設定")]
    [Tooltip("フラッシュが最大輝度になるまでの時間")]
    [SerializeField] private float fadeInDuration = 0.05f;

    [Tooltip("最大輝度を維持する時間")]
    [SerializeField] private float holdDuration = 0.05f;

    [Tooltip("フラッシュが消えるまでの時間")]
    [SerializeField] private float fadeOutDuration = 0.3f;

    [Header("暗転設定")]
    [Tooltip("画面が完全に暗くなるまでにかかる時間")]
    [SerializeField] private float fadeToBlackDuration = 1.5f;

    [Tooltip("暗転が解除されるまでにかかる時間")]
    [SerializeField] private float fadeFromBlackDuration = 1.5f;

    [Header("ホワイトアウト設定")]
    [Tooltip("画面が完全に白くなるまでにかかる時間")]
    [SerializeField] private float fadeToWhiteDuration = 2.0f;

    private Coroutine currentEffectCoroutine;

    void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 初期化
        if (flashCanvasGroup != null)
        {
            flashCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("Flash Canvas Groupが設定されていません！", this.gameObject);
        }
    }

    /// <summary>
    /// 画面フラッシュを開始する公開メソッド
    /// </summary>
    public void Flash()
    {
        if (flashCanvasGroup == null) return;

        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        currentEffectCoroutine = StartCoroutine(DoFlash());
    }

    public void FadeToBlack()
    {
        if (flashCanvasGroup == null) return;

        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        // 画面を黒くすると同時に、BGMの音量を0にする
        currentEffectCoroutine = StartCoroutine(Fade(1f, 0f, fadeToBlackDuration));
    }

    /// <summary>
    /// 画面を徐々に白くする（ホワイトアウト）公開メソッド
    /// </summary>
    public void FadeToWhite()
    {
        if (flashCanvasGroup == null) return;

        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        // 画面を白くすると同時に、BGMの音量を0にする
        currentEffectCoroutine = StartCoroutine(Fade(1f, 0f, fadeToWhiteDuration));
    }

    private IEnumerator DoFlash()
    {
        // フラッシュは視覚効果のみなので、音量は変更しない
        yield return StartCoroutine(Fade(1f, -1f, fadeInDuration));

        if (holdDuration > 0)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        yield return StartCoroutine(Fade(0f, -1f, fadeOutDuration));

        currentEffectCoroutine = null;
    }

    // Fadeメソッドに音量変更の機能を追加
    private IEnumerator Fade(float targetAlpha, float targetVolume, float duration)
    {
        IsFading = true; // フェード開始
        if (gameTimer != null)
        {
            gameTimer.PauseTimer(); // タイマーを停止
        }

        float startAlpha = flashCanvasGroup.alpha;
        float startVolume = (bgmAudioSource != null) ? bgmAudioSource.volume : -1f;
        float time = 0f;

        while (time < duration)
        {
            // 画面のアルファ値を変更
            flashCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            // BGMの音量を変更（AudioSourceが設定され、targetVolumeが0以上の場合のみ）
            if (bgmAudioSource != null && targetVolume >= 0)
            {
                bgmAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // 最終的な値を設定
        flashCanvasGroup.alpha = targetAlpha;
        if (bgmAudioSource != null && targetVolume >= 0)
        {
            bgmAudioSource.volume = targetVolume;
        }

        currentEffectCoroutine = null; // コルーチン終了時にリセット
    }
}