using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Result : MonoBehaviour
{
    // --- スコア管理関連 ---
    public static Result instance;

    [Header("スコア表示用UI")]
    [Tooltip("スコアを表示するTextMeshProのUI要素をここに登録します")]
    public TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[4];
    public static int[] scores = new int[4];

    // --- フェードイン演出関連 ---
    [Header("汎用フェードイン要素")]
    [Tooltip("背景やタイトルなど、スコア表示以外でフェードさせたいGameObjectを順番に登録します")]
    public GameObject[] generalFadeItems;

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
        // UIの初期化（透明化＆スコア表示更新）
        InitializeUI();
    }

    /// <summary>
    /// UIの初期化（アルファ値を0にし、スコアをテキストに反映）
    /// </summary>
    private void InitializeUI()
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

        // 現在のスコアをUIテキストに反映する
        for (int i = 0; i < scores.Length; i++)
        {
            UpdateScoreUI(i);
        }
    }

    /// <summary>
    /// フェードインシーケンスを開始する
    /// </summary>
    public void StartFadeIn()
    {
        // 登録されているすべての要素のフェードインを同時に開始する
        
        // 1. 汎用要素のフェードインを開始
        foreach (var obj in generalFadeItems)
        {
            if (obj != null)
            {
                var cg = obj.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    StartCoroutine(Fade(cg, fadeInDuration));
                }
            }
        }

        // 2. スコア表示のフェードインを開始
        foreach (var text in scoreTexts)
        {
            if (text != null)
            {
                var cg = text.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    StartCoroutine(Fade(cg, fadeInDuration));
                }
            }
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

    // --- スコア管理メソッド ---
    private void UpdateScoreUI(int player)
    {
        if (scoreTexts != null && player < scoreTexts.Length && scoreTexts[player] != null)
        {
            scoreTexts[player].text = "" + scores[player];
        }
    }
}