using UnityEngine;
using UnityEngine.UI;
using ZXing;  // ZXingの名前空間
using System;

public class QRCodeReader : MonoBehaviour
{
    private player_manager player_manager;
    public RawImage rawImage;
    public AudioClip player_add_sound;
    private AudioSource audioSource;
    private WebCamTexture webcamTexture;
    private IBarcodeReader barcodeReader;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player_manager = UnityEngine.Object.FindFirstObjectByType<player_manager>();

        if (WebCamTexture.devices.Length > 0)
        {
            string camName = WebCamTexture.devices[config_loader.config.camera_num].name;  // インデックス1番のカメラ名を取得
            webcamTexture = new WebCamTexture(camName);
        }
        else
        {
            Debug.LogError("カメラがありません！");
            return;
        }
        if (rawImage == null)
        {
            Debug.LogError("rawImage がセットされていません！");
            return;
        }
        rawImage.texture = webcamTexture;
        webcamTexture.Play();

        barcodeReader = new BarcodeReader();
    }

    void Update()
    {
        if (webcamTexture != null && webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            try
            {
                var snap = new Texture2D(webcamTexture.width, webcamTexture.height);
                snap.SetPixels(webcamTexture.GetPixels());
                snap.Apply();

                var result = barcodeReader.Decode(snap.GetPixels32(), snap.width, snap.height);

                if (result != null)
                {
                    Debug.Log("QRコード認識成功: " + result.Text);
                    if (int.TryParse(result.Text, out int player_id))
                    {
                        Debug.Log("正常な値を読み取りました");
                        if (!Array.Exists(player_manager.players_id, x => x == player_id))
                        {
                            audioSource.PlayOneShot(player_add_sound,1.0f);
                            player_manager.add_player(player_id);
                        }
                            

                    }
                    else
                    {
                        Debug.LogWarning("QRコードの内容が数字ではありません: " + result.Text);
                    }

                }

                Destroy(snap);
            }
            catch
            {
                // エラーは無視
            }
        }
    }
 
    public void stop_webcam()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
            webcamTexture = null;
        }
    }
}
