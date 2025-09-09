using UnityEngine;
using System;
using System.IO;
public class config_loader : MonoBehaviour
{
    public static Config config = new Config();

    void Start()
    {
        config = loadJsonData("/config.json");
        //Debug.Log(config.scene);
    }

    void Update()
    {

    }
    public Config loadJsonData(string file_name)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(Application.streamingAssetsPath + file_name);
        datastr = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<Config>(datastr);
    }
    public class Config
    {
        public string scene;
        public int camera_num;
        public string local_server_URL;
        public string DB_URL;
        public string api_key;
    }

}