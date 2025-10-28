using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class QRCodeReader : MonoBehaviour
{
    private player_manager player_manager;
    public RawImage rawImage;
    public AudioClip player_add_sound;
    private AudioSource audioSource;
    private WebCamTexture webcamTexture;
    private IBarcodeReader barcodeReader;

    // ★★★ ここから修正/追加 ★★★
    [Header("画像表示設定")]
    [Tooltip("QR認識時に表示するゲームオブジェクト（画像）のリスト。あらかじめシーンに配置し、非表示にしておくこと。")]
    public List<GameObject> imagesToActivate; // シーン内のオブジェクトを参照
    
    [Tooltip("QR認識後のクールダウン時間（秒）")]
    public float cooldownTime = 1.0f;

    private int currentImageIndex = 0; // 現在表示する画像の番号
    private bool isCooldown = false;     // クールダウン中かどうかのフラグ
    // ★★★ ここまで修正/追加 ★★★

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player_manager = UnityEngine.Object.FindFirstObjectByType<player_manager>();

        // (WebCamTextureのセットアップ ... 既存のコードのまま)
        if (WebCamTexture.devices.Length > 0)
        {
            string camName = WebCamTexture.devices[config_loader.config.camera_num].name;
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

        // ★★★ ここから追加 ★★★
        // imagesToActivateリスト内の全オブジェクトを最初に非表示にする
        if (imagesToActivate != null)
        {
            foreach (GameObject img in imagesToActivate)
            {
                if (img != null)
                {
                    img.SetActive(false);
                }
            }
        }
        // ★★★ ここまで追加 ★★★
    }

    void Update()
    {
        if (isCooldown)
        {
            return;
        }

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
                    isCooldown = true;
                    Debug.Log("QRコード認識成功: " + result.Text);
                    
                    
                    
                    audioSource.PlayOneShot(player_add_sound, 1.0f);
                    
                    StartCoroutine(CooldownRoutine());

                    // (プレイヤー追加ロジック ... 既存のコードのまま)
                    if (int.TryParse(result.Text, out int player_id))
                    {
                        Debug.Log("正常な値を読み取りました");
                        if (!Array.Exists(player_manager.players_id, x => x == player_id))
                        {
                            ShowNextImage(); // 次の画像を表示する
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

    // ★★★ ShowNextImageメソッドのロジックを書き換え ★★★
    /// <summary>
    /// リストから次のゲームオブジェクト（画像）をアクティブ（表示）にする
    /// </summary>
    private void ShowNextImage()
    {
        // 表示する画像のリストが設定されているか確認
        if (imagesToActivate == null || imagesToActivate.Count == 0)
        {
            Debug.LogWarning("imagesToActivateリストが空です。");
            return;
        }

        // 表示する画像がリストの範囲内かチェック
        if (currentImageIndex < imagesToActivate.Count)
        {
            GameObject imageToShow = imagesToActivate[currentImageIndex];

            if (imageToShow != null)
            {
                // ★★★ ここが変更点: Instantiate の代わりに SetActive(true) ★★★
                imageToShow.SetActive(true);
                Debug.Log($"画像 '{imageToShow.name}' を表示しました。");
            }
            else
            {
                Debug.LogWarning($"imagesToActivateのElement {currentImageIndex} にNullが設定されています。");
            }
            
            // 次の画像表示インデックスに進める
            currentImageIndex++;
        }
        else
        {
            // 全ての画像を表示し終わった場合の処理
            Debug.Log("全ての画像を表示し終わりました。");
            // (特に何もしない)
        }
    }
    
    /// <summary>
    /// クールダウン処理（指定時間後にフラグを戻す）
    /// </summary>
    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
        // (画像を非表示に戻す処理は、リクエストになかったためここにはありません)
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