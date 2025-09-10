using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;


public class battery_sender : MonoBehaviour
{

    public bool[] connected_wiimote = {false,false,false,false};
    public class BatteryJson
    {
        public int stage;
        public int controller_num;
        public int battery_level;
    }   
    public void send_battery_level()
    {
        for (int i = 0; i < 4; i++)
        {
            BatteryJson battery_data = new BatteryJson();
            battery_data.stage = config_loader.config.stage;
            battery_data.controller_num = i;
            if (connected_wiimote[i])
            {
                battery_data.battery_level = 100;
            }
            else
            {
                battery_data.battery_level = -1;
            }
            string json_data = JsonUtility.ToJson(log_data);
            StartCoroutine(Put(config_loader.config.local_server_URL+"battery",json_data));
            
        }


    }
    IEnumerator Put(string url, string jsonstr)
    {
    var request = new UnityWebRequest(url, "PUT");
    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);
    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    yield return request.SendWebRequest();
    }
}
