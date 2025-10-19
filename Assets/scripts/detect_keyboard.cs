
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
        if (Input.anyKey)
        {
            Debug.Log("any key pressed");
            titleVideoController.transion_to_qr();
        }
    }
}
