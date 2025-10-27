using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine;
// using UnityEngine.UI; // Textクラスは不要になるためコメントアウトまたは削除
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using TMPro; // TextMesh Proの名前空間を追加

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    
    // Text型をTextMeshProUGUI型に変更
    public TextMeshProUGUI[] scoreText = new TextMeshProUGUI[4];
    
    [Header("スコア変動演出（±○○ の表示）")]
    [Tooltip("スコア変動時に『±○○』を表示するテキスト（各プレイヤー分）")]
    public TextMeshProUGUI[] scoreDeltaText = new TextMeshProUGUI[4];
    [Tooltip("+表記の移動オフセット（AnchoredPosition で上方向に移動など）")]
    [SerializeField] private Vector2 deltaMoveOffset = new Vector2(0f, 40f);
    [Tooltip("+表記の表示時間（秒）")]
    [SerializeField] private float deltaDisplayDuration = 0.8f;
    [Tooltip("+表記の色（加点時）")]
    [SerializeField] private Color deltaPositiveColor = new Color(1f, 0.9f, 0.2f, 1f); // やや黄色
    [Tooltip("-表記の色（減点時）")]
    [SerializeField] private Color deltaNegativeColor = new Color(1f, 0.2f, 0.2f, 1f); // 赤寄り

    [Header("サウンド")]
    [Tooltip("スコア減少時に再生するサウンド")]
    public AudioClip decreaseScoreSound;
    
    public static int[] scores = new int[4];

    // 内部管理用
    private Coroutine[] deltaCoroutines = new Coroutine[4];
    private Vector2[] deltaInitialAnchoredPos = new Vector2[4];
    private bool[] deltaPosInitialized = new bool[4];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    public class ScoreJson
    {
        public int userId;
        public int score;
        public int gameSessionId;
    }

    public void AddScore(int player, int amount)
    {
        Debug.Log(player);
        scores[player] += amount;
        UpdateScoreUI(player);

        // 0 は演出を出さない
        if (amount != 0)
        {
            ShowDeltaText(player, amount); // 正の値で渡す
        }
    }

    public void DecreaseScore(int player, int amount)
    {
        scores[player] -= amount;
        if (scores[player] < 0)
        {
            scores[player] = 0;
        }
        UpdateScoreUI(player);
        Debug.Log($"Player {player} のスコアが {amount} 減少しました。現在のスコア: {scores[player]}");

        // 減点演出（0 のときは表示しない）
        if (amount > 0)
        {
            // スコア減少効果音を再生
            if (decreaseScoreSound != null && Camera.main != null)
            {
                // メインカメラの位置でサウンドを再生
                AudioSource.PlayClipAtPoint(decreaseScoreSound, Camera.main.transform.position, 1.0f);
            }
            ShowDeltaText(player, -amount); // 負の値で渡す
        }
    }
    
    /// <summary>
    /// 全てのスコア表示UIを非表示にします。
    /// </summary>
    public void HideAllScoreUI()
    {
        foreach (var text in scoreText)
        {
            if (text != null)
            {
                text.gameObject.SetActive(false);
            }
        }
    }
    
    public void send_score(int player_id, int score)
    {
        ScoreJson ScoreData = new ScoreJson();
        ScoreData.userId = player_id;
        ScoreData.score = score;
        ScoreData.gameSessionId = 1;
        string jsonstr = JsonUtility.ToJson(ScoreData);
        StartCoroutine(Post(config_loader.config.DB_URL, config_loader.config.api_key, jsonstr));
    }
    
    private void UpdateScoreUI(int player)
    {
        if (scoreText != null)
        {
            Debug.Log(player);
            // TextMeshProUGUIでも .text プロパティでテキストを設定できるため、この行は変更不要
            scoreText[player].text = "" + scores[player];
        }
    }

    private void ShowDeltaText(int player, int signedAmount)
    {
        // 安全性チェック
        if (scoreDeltaText == null) return;
        if (player < 0 || player >= scoreDeltaText.Length) return;
        var label = scoreDeltaText[player];
        if (label == null) return;

        // 初期位置を記録（最初の一回だけ）
        if (!deltaPosInitialized[player])
        {
            deltaInitialAnchoredPos[player] = label.rectTransform.anchoredPosition;
            deltaPosInitialized[player] = true;
        }

        // 進行中の演出があれば止める
        if (deltaCoroutines[player] != null)
        {
            StopCoroutine(deltaCoroutines[player]);
            deltaCoroutines[player] = null;
        }

        deltaCoroutines[player] = StartCoroutine(AnimateDelta(player, signedAmount));
    }

    private IEnumerator AnimateDelta(int player, int signedAmount)
    {
        var label = scoreDeltaText[player];
        if (label == null) yield break;

        // 文言と色設定
        label.gameObject.SetActive(true);
        int absVal = Mathf.Abs(signedAmount);
        string sign = signedAmount >= 0 ? "+" : "-";
        label.text = $"{sign}{absVal}";
        var baseColor = signedAmount >= 0 ? deltaPositiveColor : deltaNegativeColor;
        baseColor.a = 1f;
        label.color = baseColor;

        // 位置アニメーションとフェードアウト
        RectTransform rt = label.rectTransform;
        Vector2 start = deltaInitialAnchoredPos[player];
        Vector2 end = start + deltaMoveOffset;

        float t = 0f;
        while (t < deltaDisplayDuration)
        {
            float p = t / deltaDisplayDuration;
            // イーズアウトでふわっと
            float ease = 1f - Mathf.Pow(1f - p, 3f);
            rt.anchoredPosition = Vector2.LerpUnclamped(start, end, ease);
            var c = label.color;
            c.a = 1f - p;
            label.color = c;

            t += Time.deltaTime;
            yield return null;
        }

        // 終了後、元の位置に戻して隠す
        rt.anchoredPosition = start;
        label.gameObject.SetActive(false);
        deltaCoroutines[player] = null;
    }
    
    IEnumerator Post(string url, string api_key, string jsonstr)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", api_key);

        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);
    }
}