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
    public Text scoreText;          // スコア表示用
    public string message = "Stage Clear";

    void Start () {
        var img = GetComponent<Image>();
        red = img.color.r;
        green = img.color.g;
        blue = img.color.b;

        if (messageText != null)
            messageText.enabled = false;
        if (scoreText != null)
            scoreText.enabled = false;
    }

    void Update () {
        if (player == null) return;

        if (!isFading && Vector3.Distance(player.position, goalPosition) < threshold) {
            isFading = true;
            if (messageText != null) {
                messageText.enabled = true;
                messageText.text = message;
            }
            if (scoreText != null) {
                scoreText.enabled = true;
                // GameManager.Instance.score からスコアを取得
                int result = ScoreManager.score; // ScoreManagerを使用してスコアを取得
                scoreText.text = "Result : " + result;
            }
        }

        if (isFading && alfa <= 1f) {
            GetComponent<Image>().color = new Color(red, green, blue, alfa);

            if (messageText != null)
                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alfa);

            if (scoreText != null)
                scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, alfa);

            alfa += speed;
        }
    }
}