using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {
    float alfa = 0;
    float speed = 0.01f;
    float red, green, blue;
    public Transform player;
    public Vector3 goalPosition;
    public float threshold = 0.1f;
    bool isFading = false;

    public Text messageText;        // メッセージ用
    public Text[] labelText;
    public Text[] scoreText = new Text[4];          // スコア表示用
    public Text[] rankText = new Text[4];
    public Behaviour[] UI_to_delete;

    public string message = "Stage Clear";

    private int[] results = new int[4];

    void Start()
    {
        var img = GetComponent<Image>();
        red = img.color.r;
        green = img.color.g;
        blue = img.color.b;

        if (messageText != null)
            messageText.enabled = false;
        if (scoreText != null)
            foreach (var sT in scoreText)
                if (sT != null)
                    sT.enabled = false;
        if (rankText != null)
            foreach (var rT in rankText)
                if (rT != null)
                    rT.enabled = false;
        if (labelText != null)
            foreach (var lT in labelText)
                if (lT != null)
                    lT.enabled = false;
    }

    void Update () {
        if (player == null) return;

        if (!isFading && Vector3.Distance(player.position, goalPosition) < threshold)
        {
            Debug.Log("start fading");
            isFading = true;
            if (messageText != null)
            {
                messageText.enabled = true;
                messageText.text = message;
            }
            foreach (var UIToDel in UI_to_delete)
            {
                UIToDel.enabled = false;
            }

            // GameManager.Instance.score からスコアを取得
            results = ScoreManager.scores; // ScoreManagerを使用してスコアを取得
            for (int i = 0; i < 4; i++)
            {
                if (scoreText[i] != null)
                {
                    scoreText[i].enabled = true;
                    scoreText[i].text = results[i].ToString();
                }
            }

            for (int i = 0; i < 4;i++)
            {
                if(rankText[i] != null)
                    rankText[i].enabled = true;
                    string rank;
                    if (results[i] >= 85)
                    {
                        rank = "A";
                    }
                    else if (results[i] >= 70)
                    {
                        rank = "B";
                    }
                    else if (results[i] >= 60)
                    {
                        rank = "C";
                    }
                    else if (results[i] >= 50)
                    {
                        rank = "F";
                    }
                    else
                    {
                        rank = "N";
                    }
                    rankText[i].text = rank;
            }
        }
        

        if (isFading && alfa <= 1f) {
            GetComponent<Image>().color = new Color(red, green, blue, alfa);

            if (messageText != null)
                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alfa);

            if (scoreText != null)
                for (int i = 0; i < 4; i++)
                { 
                    scoreText[i].color = new Color(scoreText[i].color.r, scoreText[i].color.g, scoreText[i].color.b, alfa);    
                }
                

            alfa += speed;
        }
    }
}