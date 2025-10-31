
using UnityEngine;

public class DetectKeyboard : MonoBehaviour
{
    TitleVideoController titleVideoController;

    void Start()
    {
        titleVideoController = GetComponent<TitleVideoController>();
        if (titleVideoController == null)
        {
            Debug.LogWarning("TitleVideoController がアタッチされていません");
        }

        // プレイヤー情報をタイトルで初期化　まったく関係ない処理でごめんなさい
        ScoreManager.InitializePatchState();
        // バッテリー情報もリセット
        battery_sender.battery_send_state = 0;
        Debug.Log("初期化したよーーーーーーー19018191719813");
    }
    void Update()
    {

        // 任意のキーが押されているか
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return key pressed");
            titleVideoController.transion_to_qr();
        }
    }
}
