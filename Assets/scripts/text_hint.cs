using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 複数の時間指定テキストエントリを管理するクラス
/// </summary>
[System.Serializable]
public class TimedTextEntry
{
    [Tooltip("表示したい文字列の内容")]
    [TextArea(2, 5)]
    public string textToShow;

    [Tooltip("テキストを表示する時間（ゲーム開始からの秒数）")]
    public float displayTimeInSeconds = 5.0f;
    
    [Tooltip("テキストを表示し続ける時間（秒）")]
    public float displayDurationInSeconds = 3.0f;
}


/// <summary>
/// Inspectorで設定されたテキストリストを時間差で再生するスクリプト（フェードイン・アウト対応）
/// </summary>
public class text_hint : MonoBehaviour
{
    [Header("UI設定")]
    [Tooltip("表示するテキスト（TextMeshProUGUI）。このUIを使い回します。")]
    public TextMeshProUGUI textElement;
    
    [Header("テキストリスト")]
    [Tooltip("表示したいテキストのリストと時間設定")]
    public List<TimedTextEntry> textEntries;

    // ★★★ ここから追加 ★★★
    [Header("フェード設定")]
    [Tooltip("テキストのフェードイン/アウトにかかる時間（秒）")]
    public float fadeDuration = 0.5f;
    // ★★★ ここまで追加 ★★★

    void Start()
    {
        if (textElement == null)
        {
            Debug.LogError("textElement が設定されていません！", this);
            return;
        }

        // 最初にテキストを完全に透明にして非表示にしておく
        Color initialColor = textElement.color;
        initialColor.a = 0f;
        textElement.color = initialColor;
        textElement.gameObject.SetActive(false);
        
        if (textEntries != null)
        {
            foreach (TimedTextEntry entry in textEntries)
            {
                StartCoroutine(ShowTextRoutine(entry));
            }
        }
    }

    /// <summary>
    /// 1つのテキストエントリの表示・非表示を制御するコルーチン（フェードイン・アウト含む）
    /// </summary>
    private IEnumerator ShowTextRoutine(TimedTextEntry entry)
    {
        // 1. 指定された「表示開始時間」まで待機
        yield return new WaitForSeconds(entry.displayTimeInSeconds);

        // 2. テキストを設定し、アクティブにしてからフェードイン
        Debug.Log($"[{Time.time}秒] テキストを表示します: {entry.textToShow}");
        textElement.text = entry.textToShow;
        textElement.gameObject.SetActive(true);
        
        // ★★★ ここが変更点: フェードインを開始し、完了を待つ ★★★
        yield return StartCoroutine(FadeText(textElement, 0f, 1f, fadeDuration)); // 透明から不透明へ
        // ★★★ 変更ここまで ★★★

        // 3. 指定された「表示持続時間」だけ待機
        yield return new WaitForSeconds(entry.displayDurationInSeconds);

        // 4. テキストをフェードアウトしてから非表示にする
        // ※注意: 他のテキストがこのUIを使っている最中でも非表示にしてしまいます。
        //   表示タイミングが重ならないように設定してください。
        Debug.Log($"[{Time.time}秒] テキストを非表示にします。");
        
        // ★★★ ここが変更点: フェードアウトを開始し、完了を待つ ★★★
        yield return StartCoroutine(FadeText(textElement, 1f, 0f, fadeDuration)); // 不透明から透明へ
        // ★★★ 変更ここまで ★★★

        textElement.gameObject.SetActive(false); // フェードアウト後に非アクティブ
    }

    // ★★★ ここから新しく追加するコルーチン ★★★
    /// <summary>
    /// 指定したテキスト要素のアルファ値をスムーズに変化させるコルーチン
    /// </summary>
    /// <param name="targetText">対象のTextMeshProUGUI</param>
    /// <param name="startAlpha">開始時のアルファ値 (0f:透明, 1f:不透明)</param>
    /// <param name="endAlpha">終了時のアルファ値</param>
    /// <param name="duration">フェードにかかる時間（秒）</param>
    private IEnumerator FadeText(TextMeshProUGUI targetText, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        Color currentColor = targetText.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration; // 0から1へ進行
            currentColor.a = Mathf.Lerp(startAlpha, endAlpha, progress);
            targetText.color = currentColor;
            yield return null; // 1フレーム待機
        }

        // 確実に最終的なアルファ値に設定
        currentColor.a = endAlpha;
        targetText.color = currentColor;
    }
    // ★★★ 新しいコルーチンここまで ★★★
}