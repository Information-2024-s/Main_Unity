using UnityEngine;
using UnityEngine.SceneManagement;

public class Emergency : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // escapeキーが押されたかどうかをチェック
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!EMERGENCY KEY PRESSED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            SceneManager.LoadScene("title_scene");
        }
    }
}
