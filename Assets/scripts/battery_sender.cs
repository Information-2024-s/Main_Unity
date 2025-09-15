using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class battery_sender : MonoBehaviour
{
    public static battery_sender Instance;

    public static int[] battery_levels = { -1, -1, -1, -1 };

    void Awake()
    {
        // シーンに battery_sender が複数あると壊れるので注意
        Instance = this;
    }

    public class BatteryJson
    {
        public int stage;
        public int controller_num;
        public int battery_level;
    }

    // どこからでも呼べる static メソッド
    public static void send_battery_level()
    {
        if (Instance == null)
        {
            Debug.LogError("battery_sender Instance がシーンに存在しません！");
            return;
        }

        Instance.StartCoroutine(Instance.SendAllBatteries());
    }

    private IEnumerator SendAllBatteries()
    {
        for (int i = 0; i < 4; i++)
        {
            BatteryJson battery_data = new BatteryJson();
            battery_data.stage = config_loader.config.stage;
            battery_data.controller_num = i;
            battery_data.battery_level = battery_levels[i];
            string json_data = JsonUtility.ToJson(battery_data);
            yield return Put(config_loader.config.local_server_URL + "battery", json_data);
        }
    }

    private IEnumerator Put(string url, string jsonstr)
    {
        var request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("PUT");
        yield return request.SendWebRequest();
        
    }
}
