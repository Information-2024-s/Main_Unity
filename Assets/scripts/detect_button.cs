using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;

public class DetectButton : MonoBehaviour
{
    private Wiimote wiimote;
    [SerializeField] public int controller_num;
//    [SerializeField] private Transform firePoint;

    //public AudioClip sound1;
    private bool disconnected_log = false; //過去にログを表示したか(したならtrue)

    void Start()
    {
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
            return;
        }else if (WiimoteManager.Wiimotes.Count < controller_num + 1 && !disconnected_log){
            if(!disconnected_log){
                Debug.LogError("Wii" + controller_num + "is not connected!");
                disconnected_log = true;
            }
            return;
        }else{
            disconnected_log = false;
            wiimote = WiimoteManager.Wiimotes[controller_num];

            int ret;
            do
            {
                ret = wiimote.ReadWiimoteData();
            } while (ret > 0);



            if (wiimote.Button.b && GetComponent<RawImage>().enabled)
            {

                //AudioSource.PlayClipAtPoint(sound1, firePoint.transform.position, 1.0f);
                StartCoroutine(rumble_for(0.2f));
                TitleVideoController.transion_to_qr();
                

            }
        }

        
        
    }

    private void OnApplicationQuit()
    {
        WiimoteManager.Cleanup(wiimote);
        wiimote = null;
    }
    IEnumerator rumble_for(float seconds)
    {
            wiimote.RumbleOn = true ; // ランブルを有効にする
            wiimote.SendStatusInfoRequest(); // ステータスレポートを要求し、Rumbleを入力レポートにエンコードします
            yield return new WaitForSeconds(seconds);
            wiimote.RumbleOn = false; // ランブル無効
            wiimote.SendStatusInfoRequest(); // ステータスレポートを要求し、Rumbleを入力レポートにエンコードします
    }

}
