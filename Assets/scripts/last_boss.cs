using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events; // UnityEventを使うために必要
using UnityEngine.SceneManagement;

public class LastBoss : MonoBehaviour
{
    // シングルトンインスタンス
    public static LastBoss Instance { get; private set; }

    public bool IsFading { get; private set; } = false;

    [Header("参照")]
    [Tooltip("ホワイトアウト時に停止させたいスクリプトのリスト")]
    [SerializeField] private List<MonoBehaviour> scriptsToPause = new List<MonoBehaviour>();

    [Tooltip("表示するリザルト画面のResultスクリプト")]
    [SerializeField] private Result resultScreen;

    [Tooltip("スコア送信失敗時に表示するエラーダイアログ")]
    [SerializeField] private GameObject error_dialog; 

    [Header("UI設定")]
    [Tooltip("ホワイトアウトに使用するUIのCanvasGroup")]
    [SerializeField] private CanvasGroup whiteoutCanvasGroup;

    [Tooltip("フェードインさせたいテキストのCanvasGroup")]
    [SerializeField] private CanvasGroup textCanvasGroup;

    [Header("エフェクト設定")]
    [Tooltip("画面が白くなる/透明になるまでにかかる時間")]
    [SerializeField] private float fadeDuration = 2.0f;

    [Tooltip("ホワイトアウト後、リザルトが表示されるまでの待機時間")]
    [SerializeField] private float delayBeforeResult = 2.0f;
    [Tooltip("ノイズフェーダー。フェードから戻る（白→透明）の際にノイズ停止を呼びます。")]
    [SerializeField] private NoiseFader noiseFader;

    [Header("Skybox設定")]
    [Tooltip("ボス撃破後に切り替えるスカイボックスの Material（未指定なら変更しません）")]
    [SerializeField] private Material skyboxAfterBossDefeated;
    [Tooltip("ホワイトアウト完了時にスカイボックスを切り替えるか")]
    [SerializeField] private bool changeSkyboxOnWhiteoutComplete = true;

    [Header("イベント")]
    [Tooltip("ホワイトアウトが開始した時に呼び出されます。")]
    public UnityEvent onWhiteoutStart;

    // --- ここから追加 ---
    [Header("イベント")]
    [Tooltip("ホワイトアウトが完了した時に呼び出されます。")]
    public UnityEvent onWhiteoutComplete;
    // --- ここまで追加 ---

    private Coroutine currentEffectCoroutine;

    void Awake()
    {
        // ... (Awakeメソッドは変更なし)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (resultScreen == null)
        {
            Debug.LogError("Result Screenが設定されていません！", this.gameObject);
        }
        else
        {
            resultScreen.gameObject.SetActive(false);
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

        Debug.Log("ホワイトアウト開始イベントを実行します。");
        onWhiteoutStart.Invoke();
        
        currentEffectCoroutine = StartCoroutine(DoFade(1f, true)); // ターゲットアルファを1に
    }

    // --- ここから追加 ---
    /// <summary>
    /// 白い画面から徐々に透明にする（フェードアウト）公開メソッド
    /// </summary>
    public void FadeFromWhite()
    {
        if (whiteoutCanvasGroup == null || IsFading) return;

        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }
        currentEffectCoroutine = StartCoroutine(DoFade(0f, false)); // ターゲットアルファを0に
    }
    // --- ここまで追加 ---

    /// <summary>
    /// フェード処理とシーケンスを実行する汎用コルーチン
    /// </summary>
    /// <param name="targetAlpha">目標のアルファ値 (0 or 1)</param>
    /// <param name="isFadingToWhite">ホワイトアウト処理かどうか</param>
    private IEnumerator DoFade(float targetAlpha, bool isFadingToWhite)
    {
        IsFading = true;

        // スクリプトの状態を制御
        foreach (var script in scriptsToPause)
        {
            if (script is GameTimer timer && isFadingToWhite)
            {
                timer.PauseTimer();
            }
            else if (script != null)
            {
                script.enabled = !isFadingToWhite; // FadeToWhiteなら無効、FadeFromWhiteなら有効
            }
        }

        float time = 0f;
        float startAlpha = whiteoutCanvasGroup.alpha;

        // 1. フェード処理
        while (time < fadeDuration)
        {
            float progress = time / fadeDuration;
            whiteoutCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            if (textCanvasGroup != null) textCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            time += Time.deltaTime;
            yield return null;
        }

        whiteoutCanvasGroup.alpha = targetAlpha;
        if (textCanvasGroup != null) textCanvasGroup.alpha = targetAlpha;

        // ホワイトアウト完了時の処理
        if (isFadingToWhite)
        {

            yield return new WaitForSeconds(delayBeforeResult);

            // スカイボックスを切り替え（白画面中に行うため切り替えが視覚的に自然）
            if (changeSkyboxOnWhiteoutComplete && skyboxAfterBossDefeated != null)
            {
                RenderSettings.skybox = skyboxAfterBossDefeated;
                // 反映を即時更新（環境ライティング使用時）
                DynamicGI.UpdateEnvironment();
            }

            // --- イベントを呼び出す ---
            onWhiteoutComplete.Invoke();

            Debug.Log("ホワイトアウト完了。スコアを表示します");

            foreach (var script in scriptsToPause)
            {
                if (script is GameTimer timer && timer.timerText != null)
                {
                    timer.timerText.gameObject.SetActive(false);
                    break;
                }
            }

            if (resultScreen != null)
            {
                resultScreen.gameObject.SetActive(true);
                resultScreen.StartFadeIn();
            }

            battery_sender.send_battery_level();

            GetComponent<ScoreManager>().send_score();

            yield return new WaitForSeconds(10f);
            Debug.Log("ここまではおわってるよー");
            Debug.Log("battery_send_state"+battery_sender.battery_send_state);

            while (battery_sender.battery_send_state < 4)
            {
                Debug.Log("battery_send_state"+battery_sender.battery_send_state);
                Debug.Log("バッテリー送られてるよー");
                yield return null;
            }

            //スコア送信が全部終わるまで待ち
            while (ScoreManager.patch_state.Count(x => x != 0) != player_manager.player_count)
            {
                Debug.Log("ちょっと待ってねー");
                yield return null;
            }

            if (ScoreManager.patch_state.Sum() == player_manager.player_count)
            {
                Debug.Log("スコア表示終了。タイトルシーンへ戻ります。");
                SceneManager.LoadScene("title_scene");
            }
            else
            {
                Debug.Log("スコア送信に失敗しました。");
                error_dialog.SetActive(true);
            }
        }
        else
        {
            // フェードアウト（白->透明）処理時にノイズアニメーションを停止する
            if (noiseFader != null)
            {
                noiseFader.StartFadeOut();
            }
        }
        IsFading = false;
        currentEffectCoroutine = null;
    }
}