
using UnityEngine;

public class DetectKeyboard : MonoBehaviour
{
    TitleVideoController titleVideoController;

    void Start()
    {
        titleVideoController = GetComponent<TitleVideoController>();
        if(titleVideoController == null)
        {
            Debug.LogWarning("TitleVideoController がアタッチされていません");
        }
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
