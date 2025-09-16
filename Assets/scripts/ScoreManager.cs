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
    
    public static int[] scores = new int[4];

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