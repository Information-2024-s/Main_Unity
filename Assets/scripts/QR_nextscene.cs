using UnityEngine;
using UnityEngine.SceneManagement;

public class QR_nextscene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        // 任意のキーが押されているか
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return key pressed");
            SceneManager.LoadScene("Wave "+(config_loader.config.stage+1));
        }
    }
}
