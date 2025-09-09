using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;


public class log_sender : MonoBehaviour
{

    public class BatteryJson
    {
        public int stage;
        public int controller_num;
        public int battery_level;
    }   
    public void send_battery_level{
        BatteryJson battery_data = new BatteryJson();
        battery_data.stage = config_loader.config.stage;
        
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
