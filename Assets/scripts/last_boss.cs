using UnityEngine;
using System.Collections;

public class LastBoss : MonoBehaviour
{
    // シングルトンインスタンス
    public static LastBoss Instance { get; private set; }

    /// <summary>
    /// 現在、画面効果（ホワイトアウト）が実行中かどうか
    /// </summary>
    public bool IsFading { get; private set; } = false;

    [Header("参照")]
    [Tooltip("停止させたいGameTimerスクリプト")]
    [SerializeField] private GameTimer gameTimer;

    [Tooltip("フェードアウトさせたいBGMのAudioSource")]
    [SerializeField] private AudioSource bgmAudioSource;

    [Header("UI設定")]
    [Tooltip("ホワイトアウトに使用するUIのCanvasGroup")]
    [SerializeField] private CanvasGroup whiteoutCanvasGroup;

    [Tooltip("フェードインさせたいテキストのCanvasGroup")]
    [SerializeField] private CanvasGroup textCanvasGroup;

    [Header("ホワイトアウト設定")]
    [Tooltip("画面が完全に白くなるまでにかかる時間")]
    [SerializeField] private float fadeToWhiteDuration = 2.0f;

    [Tooltip("フェード終了後、シーン遷移までの待機時間（秒）")]
    [SerializeField] private float delayBeforeSceneChange = 10.0f;

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

        if (whiteoutCanvasGroup != null)
        {
            whiteoutCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("Whiteout Canvas Groupが設定されていません！", this.gameObject);
        }

        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// 画面を徐々に白くする（ホワイトアウト）公開メソッド
    /// </summary>
    public void FadeToWhite()
    {
        if (whiteoutCanvasGroup == null || IsFading) return;

        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        currentEffectCoroutine = StartCoroutine(DoFadeToWhite());
    }

    /// <summary>
    /// ホワイトアウトとBGMフェードアウトを実行するコルーチン
    /// </summary>
    private IEnumerator DoFadeToWhite()
    {
        // IsFadingをtrueにすることで、Syouzyun.cs側で射撃が停止されます
        IsFading = true;

        // タイマーを停止
        if (gameTimer != null)
        {
            gameTimer.PauseTimer();
        }

        float time = 0f;
        float startAlpha = whiteoutCanvasGroup.alpha;
        float startVolume = (bgmAudioSource != null) ? bgmAudioSource.volume : 0f;
        float textStartAlpha = (textCanvasGroup != null) ? textCanvasGroup.alpha : 0f;

        while (time < fadeToWhiteDuration)
        {
            float progress = time / fadeToWhiteDuration;

            // 画面のアルファ値を変更 (ホワイトアウト)
            whiteoutCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, progress);

            // テキストのアルファ値を変更 (フェードイン)
            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = Mathf.Lerp(textStartAlpha, 1f, progress);
            }

            // BGMの音量を変更 (フェードアウト)
            if (bgmAudioSource != null)
            {
                bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, progress);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // 最終的な値を設定
        whiteoutCanvasGroup.alpha = 1f;
        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 1f;
        }
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = 0f;
        }

        // IsFadingをfalseに戻すことで、再び射撃が可能になります　多分必要ないのでコメントアウト

        //IsFading = false;
        currentEffectCoroutine = null;

        // 指定した時間待機
        yield return new WaitForSeconds(delayBeforeSceneChange);

        // シーンを遷移
        Debug.Log("QRリーダーへ移動します");
        //SceneManager.LoadScene("QR_read");
    }
}