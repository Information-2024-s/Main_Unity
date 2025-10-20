using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // OrderBy を使うために必要
using TMPro;

// [System.Serializable] をつけることで、インスペクター上に表示・編集できるようになる
[System.Serializable]
public class RankTier
{
    [Tooltip("ランク名（例: S, A, B, C）")]
    public string rankName;
    [Tooltip("このランクになるために必要な最低スコア")]
    public int minimumScore;
}

public class Result : MonoBehaviour
{
    // --- スコア管理関連 ---
    public static Result instance;

    [Header("スコア・ランク表示用UI")]
    [Tooltip("スコアを表示するTextMeshProのUI要素をここに登録します")]
    public TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[4];
    [Tooltip("ランクを表示するTextMeshProのUI要素をここに登録します")]
    public TextMeshProUGUI[] rankTexts = new TextMeshProUGUI[4];

    // --- フェードイン演出関連 ---
    [Header("汎用フェードイン要素")]
    [Tooltip("背景やタイトルなど、スコア・ランク以外でフェードさせたいGameObjectを順番に登録します")]
    public GameObject[] generalFadeItems;

    [Header("ランク設定")]
    [Tooltip("スコアの段階を設定します。スコアが高い順に並べてください（例: S, A, B, C）")]
    public List<RankTier> rankTiers = new List<RankTier>();

    [Header("フェード設定")]
    [Tooltip("すべての要素がフェードインするのにかかる時間")]
    [SerializeField] private float fadeInDuration = 1.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // UIを透明にする初期化のみ行う
        InitializeUIAppearance();
    }

    /// <summary>
    /// UIの見た目を初期化（アルファ値を0に）する
    /// </summary>
    private void InitializeUIAppearance()
    {
        // 汎用要素をすべて透明にする
        foreach (var obj in generalFadeItems)
        {
            if (obj != null)
            {
                var cg = obj.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0f;
            }
        }

        // スコア表示テキストをすべて透明にする
        foreach (var text in scoreTexts)
        {
            if (text != null)
            {
                var cg = text.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0f;
            }
        }
        
        // ランク表示テキストをすべて透明にする
        foreach (var text in rankTexts)
        {
            if (text != null)
            {
                var cg = text.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0f;
            }
        }
    }

    /// <summary>
    /// フェードインシーケンスを開始する
    /// </summary>
    public void StartFadeIn()
    {
        // --- 表示直前にスコアとランクを更新 ---
        UpdateAllUIContents();

        // Safety: StartCoroutine を呼ぶ前にこのコンポーネントと GameObject が有効か確認する
        if (!this.isActiveAndEnabled || this.gameObject == null || !this.gameObject.activeInHierarchy)
        {
            // GameObject が無効なら有効化してからフェードインを開始する
            // 注意: 他のロジックで無効化している可能性があるため、強制的に有効化するのが望ましくない場合は
            // 呼び出し側で有効化してから StartFadeIn を呼ぶようにしてください。
            this.gameObject.SetActive(true);
        }

        // 登録されているすべての要素のフェードインを同時に開始する
        
        // 1. 汎用要素のフェードインを開始
        foreach (var obj in generalFadeItems)
        {
            if (obj == null) continue;
            var cg = obj.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                // Ensure target object is active so CanvasGroup updates are visible
                if (!obj.activeInHierarchy) obj.SetActive(true);
                StartCoroutine(Fade(cg, fadeInDuration));
            }
        }

        // 2. スコア表示のフェードインを開始
        foreach (var text in scoreTexts)
        {
            if (text == null) continue;
            var cg = text.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                if (!text.gameObject.activeInHierarchy) text.gameObject.SetActive(true);
                StartCoroutine(Fade(cg, fadeInDuration));
            }
        }
        
        // 3. ランク表示のフェードインを開始
        foreach (var text in rankTexts)
        {
            if (text == null) continue;
            var cg = text.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                if (!text.gameObject.activeInHierarchy) text.gameObject.SetActive(true);
                StartCoroutine(Fade(cg, fadeInDuration));
            }
        }
    }

    /// <summary>
    /// UIのテキスト内容を最新のスコアで更新する
    /// </summary>
    private void UpdateAllUIContents()
    {
        for (int i = 0; i < ScoreManager.scores.Length; i++)
        {
            UpdateScoreUI(i);
            UpdateRankUI(i);
        }
    }

    /// <summary>
    /// 指定されたCanvasGroupのアルファ値を0から1へ徐々に変更するコルーチン
    /// </summary>
    private IEnumerator Fade(CanvasGroup canvasGroup, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    // --- スコア・ランク更新メソッド ---
    private void UpdateScoreUI(int player)
    {
        if (scoreTexts != null && player < scoreTexts.Length && scoreTexts[player] != null)
        {
            scoreTexts[player].text = "" + ScoreManager.scores[player];
        }
    }

    private void UpdateRankUI(int player)
    {
        if (rankTexts != null && player < rankTexts.Length && rankTexts[player] != null)
        {
            int currentScore = ScoreManager.scores[player];
            string rank = GetRank(currentScore);
            rankTexts[player].text = rank;
        }
    }

    /// <summary>
    /// スコアに応じたランク名を取得する
    /// </summary>
    private string GetRank(int score)
    {
        // rankTiersをスコアの高い順にソートしてから評価する
        foreach (var tier in rankTiers.OrderByDescending(t => t.minimumScore))
        {
            if (score >= tier.minimumScore)
            {
                return tier.rankName; // 条件に一致した最初のランクを返す
            }
        }
        // どのランクにも満たない場合はデフォルトのランクを返す（例: "D" や ""）
        return "D"; 
    }
}