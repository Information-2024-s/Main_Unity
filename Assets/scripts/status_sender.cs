using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;


public class status_sender : MonoBehaviour
{

    public class LogJson
    {
        public int stage;
        public string log;
        public int logger_level;
    }
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)//logが出力されるたびにサーバーに送信
    {
        LogJson log_data = new LogJson();
        log_data.stage = 1;
        log_data.log = logString;
        switch (type)//ロガーレベルを設定(小さいほうが重要)
        {
            case LogType.Error:
                log_data.logger_level = 0;
                break;
            case LogType.Assert:
                log_data.logger_level = 0;
                break;
            case LogType.Warning:
                log_data.logger_level = 1;
                break;
            case LogType.Log:
                log_data.logger_level = 2;
                break;
            case LogType.Exception:
                log_data.logger_level = 0;
                break;
            default:
                log_data.logger_level = 0;
                break;
        }
        string json_data = JsonUtility.ToJson (log_data);
        StartCoroutine(Post(config_loader.config.local_server_URL+"log",json_data));
    }
    IEnumerator Post(string url, string jsonstr)
    {
        var request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
    }
}
