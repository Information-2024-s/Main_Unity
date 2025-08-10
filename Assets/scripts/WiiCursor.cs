using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;

public class WiimoteTest : MonoBehaviour
{
    private Wiimote wiimote;
    private int flag = 0;
    [SerializeField] public int controller_num;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float power = 500f;
    public float cooltime = 0.2f;
    private float timer = 0f;
    public AudioClip sound1;

    void Start()
    {
        WiimoteManager.FindWiimotes();
    }

    void Update()
    {

        timer += Time.deltaTime;

        if (!WiimoteManager.HasWiimote())
        {
            Debug.Log("Wii is not connected!");
            return;
        }
        else if (WiimoteManager.Wiimotes.Count < controller_num + 1)
        {
            Debug.Log("Wii" + controller_num + "is not connected!");
            return;
        }

        wiimote = WiimoteManager.Wiimotes[controller_num];

        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();
        } while (ret > 0);

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


        if (wiimote.Button.b && timer >= cooltime && GetComponent<RawImage>().enabled)
        {
            timer = 0f;
            Vector3 screenPos = new Vector3(pointer[0] * 1920, pointer[1] * 1080, 0);
            //Debug.Log(screenPos);
            //Debug.Log("mouse: " + Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            GameObject bulletObj = Instantiate(bullet, firePoint.position, Quaternion.identity);
            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
            AudioSource.PlayClipAtPoint(sound1, firePoint.transform.position, 1.0f);
            StartCoroutine(rumble_for(0.2f));

            if (rb != null)
            {
                rb.AddForce(ray.direction * power, ForceMode.Impulse);
            }
            //Debug.Log("弾の位置: " + bulletObj.transform.position);
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
