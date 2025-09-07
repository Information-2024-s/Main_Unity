using UnityEngine;
using TMPro; // 追加

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI[] scoreText = new TextMeshProUGUI[4]; // 型を変更
    public static int[] scores = new int[4];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddScore(int player,int amount)
    {
        Debug.Log(player);
        scores[player] += amount;
        UpdateScoreUI(player);
    }

    private void UpdateScoreUI(int player)
    {
        if (scoreText != null)
        {
            Debug.Log(player);
            scoreText[player].text = "" + scores[player];
        }
    }

}