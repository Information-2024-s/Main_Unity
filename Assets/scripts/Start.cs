using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    float alfa = 1f;
    public float speed = 0.5f; // 秒間減少量
    float red, green, blue;
    public Transform player;
    public Vector3 targetPosition;
    public float threshold = 5f; // 少し大きめ
    bool isFading = false;

    public Text messageText;
    public Text targetText;
    public Text targetEnemyText;
    public string message = "WAVE 1";
    public string target = "敵を倒せ";
    public string target_enemy = "目標撃破数：10体";

    public GameObject shoterObject;

    void Start()
    {
        var img = GetComponent<Image>();
        red = img.color.r;
        green = img.color.g;
        blue = img.color.b;

        shoterObject.GetComponent<Syouzyun>().enabled = false;

        if (messageText != null)
        {
            messageText.enabled = true;
            messageText.text = message;
        }
        if (targetText != null)
        {
            targetText.enabled = true;
            targetText.text = target;
        }
        if (targetEnemyText != null)
        {
            targetEnemyText.enabled = true;
            targetEnemyText.text = target_enemy;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 距離が近づいたらフェード開始
        if (!isFading && Vector3.Distance(player.position, targetPosition) < threshold)
        {
            isFading = true;
            shoterObject.GetComponent<Syouzyun>().enabled = true;
        }

        // フェード処理
        if (isFading && alfa > 0f)
        {
            alfa = Mathf.Max(alfa - speed * Time.deltaTime, 0f);
            GetComponent<Image>().color = new Color(red, green, blue, alfa);

            if (messageText != null)
                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alfa);
            if (targetText != null)
                targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, alfa);
            if (targetEnemyText != null)
                targetEnemyText.color = new Color(targetEnemyText.color.r, targetEnemyText.color.g, targetEnemyText.color.b, alfa);
        }
    }
}
