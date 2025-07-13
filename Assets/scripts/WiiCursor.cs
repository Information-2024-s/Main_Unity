using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;

public class WiimoteTest : MonoBehaviour
{
    public RectTransform ir_pointer0;

    private Wiimote wiimote0;
    private int flag = 0;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float power = 500f;
    public float cooltime = 0.2f;

    private float timer = 0f;

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

        wiimote0 = WiimoteManager.Wiimotes[0];

        int ret0;
        do
        {
            ret0 = wiimote0.ReadWiimoteData();
        } while (ret0 > 0);

        float[] pointer0 = wiimote0.Ir.GetPointingPosition();

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(pointer0[0], pointer0[1]);
        rt.anchorMax = new Vector2(pointer0[0], pointer0[1]);
        //Debug.Log(pointer0);

        if (flag == 0)
        {
            wiimote0.SetupIRCamera(IRDataType.BASIC);
            flag++;
        }


        if (wiimote0.Button.b && timer >= cooltime)
        {
            timer = 0f;
            Vector3 screenPos = new Vector3(pointer0[0] * 1920, pointer0[1] * 1080, 0);
            //Debug.Log(screenPos);
            //Debug.Log("mouse: " + Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            GameObject bulletObj = Instantiate(bullet, firePoint.position, Quaternion.identity);
            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(ray.direction * power, ForceMode.Impulse);
            }
            //Debug.Log("弾の位置: " + bulletObj.transform.position);
        }
}

    private void OnApplicationQuit()
    {
        WiimoteManager.Cleanup(wiimote0);
        wiimote0 = null;
    }

}