using UnityEngine;
using UnityEngine.SceneManagement;

public class QR_nextscene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private QRCodeReader QRCodeReader;
    void Start()
    {
        QRCodeReader = UnityEngine.Object.FindFirstObjectByType<QRCodeReader>();
    }

    void Update()
    {
        // 任意のキーが押されているか
        if (Input.GetKeyDown(KeyCode.Return))
        {
            QRCodeReader.stop_webcam();
            Debug.Log("Return key pressed");
            SceneManager.LoadScene("Wave "+(config_loader.config.stage+1));
        }
    }
}
