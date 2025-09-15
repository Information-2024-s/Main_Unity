using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Goal : MonoBehaviour
{
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
        
    }

    void Update()
    {
        if (player == null) return;

        if (!isFading && Vector3.Distance(player.position, goalPosition) < threshold)
        {
            battery_sender.send_battery_level();
            results = ScoreManager.scores; // ScoreManagerを使用してスコアを取得
            for (int i = 0; i < player_manager.player_count; i++)
            {
                ScoreManager.instance.send_score(player_manager.players_id[i], results[i]);
            }
            Debug.Log("start fading");
            isFading = true;
            if (messageText != null)
            {
                messageText.enabled = true;
                messageText.text = message;
            }
            foreach (var lT in labelText)
            {
                lT.enabled = true;
            }
            foreach (var UIToDel in UI_to_delete)
            {
                UIToDel.enabled = false;
            }


            int tmp_score = 0;
            for (int i = 0; i < 4; i++)
            {
                if (scoreText[i] != null)
                {
                    scoreText[i].enabled = true;
                    scoreText[i].text = tmp_score.ToString();
                    StartCoroutine(ScoreAnimation(scoreText[i], results[i], 2));
                }
            }


            for (int i = 0; i < 4; i++)
            {
                if (rankText[i] != null)
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



        if (isFading && alfa <= 1f)
        {
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
    IEnumerator ScoreAnimation(Text text_component,float addScore, float time)
    {
        float score = Convert.ToSingle(text_component.text);
        //前回のスコア
        float befor = score;
        //今回のスコア
        float after = score + addScore;
        //得点加算
        score += addScore;
        //0fを経過時間にする
        float elapsedTime = 0.0f;

        //timeが０になるまでループさせる
        while (elapsedTime < time)
        {
            float rate = elapsedTime / time;
            // テキストの更新
            text_component.text = (befor + (after - befor) * rate).ToString("f0");

            elapsedTime += Time.deltaTime;
            // 0.01秒待つ
            yield return new WaitForSeconds(0.01f);
        }
        // 最終的な着地のスコア
        text_component.text = after.ToString();
    }
}