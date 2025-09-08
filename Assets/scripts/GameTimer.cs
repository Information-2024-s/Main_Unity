using UnityEngine;
using TMPro; // TextMeshProを扱うために必要
using System.Collections.Generic; // Listを使うために必要
using System.Collections; // コルーチンを使うために必要
using UnityEngine.Events; // UnityEventを使うために必要
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("制限時間リスト（秒）")]
    [Tooltip("実行したいタイマーの制限時間を順番に設定します。")]
    public List<float> timeLimitsInSeconds = new List<float> { 60f };

    [Header("タイマー間の待機時間リスト（秒）")]
    [Tooltip("各タイマーの後に次のタイマーが開始するまでの待機時間。要素数は「制限時間リスト」の数より1つ少なく設定してください。")]
    public List<float> delaysBetweenTimers = new List<float>();

    [Header("時間を表示するUIテキスト")]
    [Tooltip("シーン内のTextMeshProオブジェクトをここにドラッグ＆ドロップしてください")]
    public TextMeshProUGUI timerText;

    [Header("待機中のイベント")]
    [Tooltip("待機時間が開始する時に呼び出されます。")]
    public UnityEvent onWaitStart;

    [Tooltip("待機時間が終了した時に呼び出されます。")]
    public UnityEvent onWaitEnd;

    private bool isPaused = false; // タイマーが一時停止中かどうかのフラグ

    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static GameTimer Instance { get; private set; }

    /// <summary>
    /// 現在、タイマー間の待機中かどうか
    /// </summary>
    public bool IsWaiting { get; private set; } = false;

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

    void Start()
    {
        // UIテキストが設定されているか確認
        if (timerText == null)
        {
            Debug.LogError("TimerTextが設定されていません！インスペクターでUIテキストをアタッチしてください。");
            return;
        }

        // 制限時間が設定されているか確認
        if (timeLimitsInSeconds.Count == 0)
        {
            Debug.LogWarning("制限時間が設定されていません。");
            UpdateTimerDisplay(0);
            return;
        }

        // タイマーシーケンスを開始
        StartCoroutine(TimerSequence());
    }

    /// <summary>
    /// 複数のタイマーを順番に実行するコルーチン
    /// </summary>
    private IEnumerator TimerSequence()
    {
        for (int i = 0; i < timeLimitsInSeconds.Count; i++)
        {
            // 2つ目以降のタイマーの場合、前に待機時間を設ける
            if (i >= 0)
            {
                // 待機時間が設定されているか確認 (delaysBetweenTimersのインデックスは i - 1)
                if (i <= delaysBetweenTimers.Count)
                {
                    float delay = delaysBetweenTimers[i];
                    Debug.Log($"次のタイマーまで {delay}秒 待機します。");

                    // --- 待機開始イベントを呼び出す ---
                    IsWaiting = true; // 待機状態を開始
                    onWaitStart.Invoke();

                    // 指定された秒数だけ待機
                    yield return new WaitForSeconds(delay);

                    // --- 待機終了イベントを呼び出す ---
                    IsWaiting = false; // 待機状態を終了
                    onWaitEnd.Invoke();
                }
                else
                {
                    Debug.LogWarning($"タイマー #{i} の前の待機時間が設定されていません。待機せずに次のタイマーを開始します。");
                }
            }

            // 現在のタイマーの残り時間を設定
            float remainingTime = timeLimitsInSeconds[i];
            Debug.Log($"タイマー #{i + 1} を開始します。制限時間: {remainingTime}秒");

            // 残り時間が0になるまでループ
            while (remainingTime > 0)
            {
                // isPausedがtrueの間は、ここで処理が一時停止する
                while (isPaused)
                {
                    yield return null;
                }

                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay(remainingTime);
                yield return null; // 1フレーム待つ
            }

            // 時間切れの処理
            remainingTime = 0;
            UpdateTimerDisplay(remainingTime);
            Debug.Log($"タイマー #{i + 1} が時間切れになりました！");
        }

        float finalDelay = delaysBetweenTimers[timeLimitsInSeconds.Count];
        Debug.Log($"次のタイマーまで {finalDelay}秒 待機します。");

        // --- 待機開始イベントを呼び出す ---
        IsWaiting = true; // 待機状態を開始
        onWaitStart.Invoke();

        // 指定された秒数だけ待機
        yield return new WaitForSeconds(finalDelay);

        // --- 待機終了イベントを呼び出す ---
        IsWaiting = false; // 待機状態を終了
        onWaitEnd.Invoke();

        Debug.Log("全てのタイマーが終了しました。QRリーダーへ移動します");
        //SceneManager.LoadScene("QR_read");
    }

    /// <summary>
    /// タイマーを一時停止する公開メソッド
    /// </summary>
    public void PauseTimer()
    {
        isPaused = true;
    }

    /// <summary>
    /// タイマーを再開する公開メソッド
    /// </summary>
    public void ResumeTimer()
    {
        isPaused = false;
    }

    /// <summary>
    /// UIのテキスト表示を更新するメソッド
    /// </summary>
    /// <param name="timeToDisplay">表示する時間（秒）</param>
    private void UpdateTimerDisplay(float timeToDisplay)
    {
        // ... (変更なし)
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}