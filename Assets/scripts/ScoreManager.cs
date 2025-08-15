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
    public class ScoreJson {
        public int player_id;
        public int score;
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
        ScoreData.player_id = player_id;
        ScoreData.score = score;
        string jsonstr = JsonUtility.ToJson (ScoreData);
        StartCoroutine(Post(config_loader.config.DB_URL, jsonstr));
    }
    private void UpdateScoreUI(int player)
    {
        if (scoreText != null)
        {
            Debug.Log(player);
            scoreText[player].text = "" + scores[player];
        }
    }
    IEnumerator Post(string url, string jsonstr)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);
    }

}
