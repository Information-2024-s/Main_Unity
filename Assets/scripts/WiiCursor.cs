using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using UnityEngine.EventSystems;  
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class WiiCursor : MonoBehaviour
{
    private Wiimote wiimote;
    private int flag = 0;
    public int controller_num;
    public AudioClip sound1;
    public GameObject shot_me_object;
    public bool transion_for_debug = false;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    private QRCodeReader QRCodeReader;
    private VideoPlayer videoPlayer;
    private bool disconnected_log = false; //過去にログを表示したか(したならtrue)
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayerコンポーネントがありません！");
            return;
        }

        videoPlayer.time = 0.01f;  // 再生開始位置セット
        videoPlayer.Pause();       // 一旦停止（再生はしない

        QRCodeReader = UnityEngine.Object.FindFirstObjectByType<QRCodeReader>();
        raycaster = GetComponentInParent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        WiimoteManager.FindWiimotes();
    }

    void Update()
    {
        if (!WiimoteManager.HasWiimote())
        {
            if(!disconnected_log){
                Debug.LogError("Wii is not connected!");
                disconnected_log = true;
            }
            battery_sender.battery_levels[controller_num] = -1;
            return;
        }else if (WiimoteManager.Wiimotes.Count < controller_num + 1 && !disconnected_log){
            if(!disconnected_log){
                Debug.LogError("Wii" + controller_num + "is not connected!");
                disconnected_log = true;
            }
            battery_sender.battery_levels[controller_num] = -1;
            return;
        }else{
            disconnected_log = false;
            wiimote = WiimoteManager.Wiimotes[controller_num];

            int ret;
            do
            {
                ret = wiimote.ReadWiimoteData();
            } while (ret > 0);
            battery_sender.battery_levels[controller_num] = wiimote.Status.battery_level*100/255;
            Debug.Log(battery_sender.battery_levels[controller_num]);

            float[] pointer = wiimote.Ir.GetPointingPosition();

            RectTransform rt = GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(pointer[0], pointer[1]);
            rt.anchorMax = new Vector2(pointer[0], pointer[1]);
            //Debug.Log(pointer0);

            if (flag == 0)
            {
                wiimote.SetupIRCamera(IRDataType.BASIC);
                flag++;
            }


            if (wiimote.Button.b && GetComponent<RawImage>().enabled)
            {
                Vector3 screenPos = new Vector3(pointer[0] * 1920, pointer[1] * 1080, 0);

                //AudioSource.PlayClipAtPoint(sound1, firePoint.transform.position, 1.0f);
                StartCoroutine(rumble_for(0.2f));


                Vector2 cursorPos = new Vector2(screenPos.x, screenPos.y);
                PointerEventData pointerData = new PointerEventData(eventSystem);
                pointerData.position = cursorPos;

                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerData, results);

                if (ContainsGameObject(results, shot_me_object))
                {
                    StartCoroutine(transion_next_scene());
                }


            }
            if (transion_for_debug)
            {
                transion_for_debug = false;
                StartCoroutine(transion_next_scene());
            }
        }
    }

    private void OnApplicationQuit()
    {
        WiimoteManager.Cleanup(wiimote);
        wiimote = null;
    }
    private bool ContainsGameObject(List<RaycastResult> results, GameObject target)
    {
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == target)
            {
                return true;
            }
        }
        return false;
    }
    IEnumerator rumble_for(float seconds)
    {
        wiimote.RumbleOn = true; // ランブルを有効にする
        wiimote.SendStatusInfoRequest(); // ステータスレポートを要求し、Rumbleを入力レポートにエンコードします
        yield return new WaitForSeconds(seconds);
        wiimote.RumbleOn = false; // ランブル無効
        wiimote.SendStatusInfoRequest(); // ステータスレポートを要求し、Rumbleを入力レポートにエンコードします
    }
    IEnumerator transion_next_scene()
    {
        wiimote.RumbleOn = false; // ランブル無効
        wiimote.SendStatusInfoRequest(); // ステータスレポートを要求し、Rumbleを入力レポートにエンコードします
        QRCodeReader.stop_webcam();
        videoPlayer.Play();
        yield return new WaitWhile(() => videoPlayer.isPlaying);
        SceneManager.LoadScene(config_loader.config.scene_name);
    }

}
