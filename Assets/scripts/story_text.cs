using UnityEngine;
using TMPro; // TextMeshProを扱うために必要
using System.Collections;
using System.Collections.Generic; // Listを使うために必要

// CanvasGroupコンポーネントを必須にする
[RequireComponent(typeof(CanvasGroup))]
public class SequentialTextDisplayer : MonoBehaviour
{
    [Header("UI設定")]
    [Tooltip("テキストを表示するTextMeshProオブジェクト")]
    public TextMeshProUGUI displayText;

    [Header("表示テキストリスト")]
    [Tooltip("待機時間ごとに順番に表示したいテキストをここに入力します。")]
    public List<string> textsToShow = new List<string>();

    [Header("フェード設定")]
    [Tooltip("テキストがフェードイン/アウトする時間（秒）")]
    public float fadeDuration = 0.5f;

    // --- 内部変数 ---
    private CanvasGroup canvasGroup;
    private int currentIndex = 0; // 次に表示するテキストのインデックス

    void Awake()
    {
        // 必要なコンポーネントを取得
        canvasGroup = GetComponent<CanvasGroup>();

        // displayTextが設定されていなければ、自身の子から探す
        if (displayText == null)
        {
            displayText = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (displayText == null)
        {
            Debug.LogError("表示用のTextMeshProオブジェクトが見つかりません！", this.gameObject);
            return;
        }

        // 初期状態では透明にしておく
        canvasGroup.alpha = 0f;
        displayText.text = "";
    }

    /// <summary>
    /// 次のテキストをフェードインで表示する公開メソッド
    /// </summary>
    public void DisplayNextText()
    {
        // 表示するテキストがなければ何もしない
        if (textsToShow.Count == 0) return;

        // リストの範囲を超えないようにインデックスを調整
        if (currentIndex >= textsToShow.Count)
        {
            Debug.LogWarning("全てのテキストを表示し終わりました。最初に戻ります。");
            currentIndex = 0;
        }

        // テキストを更新してフェードインを開始
        displayText.text = textsToShow[currentIndex];
        StartCoroutine(Fade(1f));

        // 次の呼び出しのためにインデックスを更新
        currentIndex++;
    }

    /// <summary>
    /// テキストをフェードアウトで非表示にする公開メソッド
    /// </summary>
    public void HideText()
    {
        StartCoroutine(Fade(0f));
    }

    /// <summary>
    /// 指定されたアルファ値へ徐々に変化させるコルーチン
    /// </summary>
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}