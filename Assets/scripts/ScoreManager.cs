using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public Text[] scoreText = new Text[4];
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
    public void send_score(int player_id,int score)
    {
        ScoreJson ScoreData = new ScoreJson();
        ScoreData.userId = player_id;
        ScoreData.score = score;
        ScoreData.gameSessionId = 1;
        string jsonstr = JsonUtility.ToJson (ScoreData);
        StartCoroutine(Post(config_loader.config.DB_URL,config_loader.config.api_key, jsonstr));
    }
    private void UpdateScoreUI(int player)
    {
        if (scoreText != null)
        {
            Debug.Log(player);
            scoreText[player].text = "" + scores[player];
        }
    }
    IEnumerator Post(string url,string api_key, string jsonstr)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", api_key);

        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);
    }

}