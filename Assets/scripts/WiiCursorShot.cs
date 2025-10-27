using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;

public class WiiCursorShot : MonoBehaviour
{
    private Wiimote wiimote;
    private int flag = 0;
    private bool isIRSetupComplete = false;
    [SerializeField] public int controller_num;
    [SerializeField] public bool useMotionPlus = false; // インスペクターでモーションプラス使用を指定
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float power = 500f;
    public float cooltime = 1.0f;
    private float timer = 0f;
    public AudioClip sound1;
    private bool disconnected_log = false; //過去にログを表示したか(したならtrue)

    void Start()
    {
        WiimoteManager.FindWiimotes();
        int playerCount = Mathf.Max(1, player_manager.player_count);
    }

    void Update()
    {

        timer += Time.deltaTime;

        // --- 接続チェック (バグ修正済み) ---
        if (!WiimoteManager.HasWiimote())
        {
            if(!disconnected_log){
                Debug.LogError("Wii is not connected!");
                disconnected_log = true;
            }
            return;
        }
        // ★★★ 以前指摘したバグの修正箇所 ★★★
        // 「&& !disconnected_log」 を削除し、コントローラーが足りない場合は
        // ログの状態に関わらず、必ず return するように修正
        else if (WiimoteManager.Wiimotes.Count < controller_num + 1)
        {
            if(!disconnected_log){
                Debug.LogError("Wii" + controller_num + "is not connected!");
                disconnected_log = true;
            }
            return;
        }
        // --- 接続チェックここまで ---
        else
        {
            disconnected_log = false;
            wiimote = WiimoteManager.Wiimotes[controller_num];

            int ret;
            do
            {
                ret = wiimote.ReadWiimoteData();
            } while (ret > 0);

            float[] pointer = wiimote.Ir.GetPointingPosition();
            
            // IRデータのデバッグログ（一時的）
            if (Time.frameCount % 60 == 0) // 60フレームに1回ログ出力
            {
                Debug.Log("IR pointer: [" + pointer[0] + ", " + pointer[1] + "]");
            }

            RectTransform rt = GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(pointer[0], pointer[1]);
            rt.anchorMax = new Vector2(pointer[0], pointer[1]);
            //Debug.Log(pointer0);

            if (flag == 0)
            {
                // IRカメラのセットアップをコルーチンで実行
                StartCoroutine(SetupIRCameraWithDelay());
                flag++;
            }

            // IRセットアップが完了するまで待機
            if (!isIRSetupComplete)
            {
                return;
            }


            if (wiimote.Button.b && timer >= cooltime && GetComponent<RawImage>().enabled)
            {

                // LastBossがフェード中の場合は、ここで処理を中断して射撃しない
                if (LastBoss.Instance != null && LastBoss.Instance.IsFading)
                {
                    return;
                }
                // --- ここまで追加 ---

                // GameTimerが待機中の場合は射撃しない
                if (GameTimer.Instance != null && GameTimer.Instance.IsWaiting)
                {
                    return;
                }
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

    IEnumerator SetupIRCameraWithDelay()
{
    Debug.Log("========== IRカメラセットアップ開始 ==========");
    Debug.Log("コントローラー番号: " + controller_num);
    Debug.Log("モーションプラス使用設定: " + (useMotionPlus ? "有効 (Wiiリモコンプラス)" : "無効 (通常のWiiリモコン)"));
    
    if (useMotionPlus)
    {
        // モーションプラス使用時の処理
        Debug.Log(">> Wiiリモコンプラス用のセットアップを実行");
        
        // モーションプラスの識別を要求
        Debug.Log(">> RequestIdentifyWiiMotionPlus() 呼び出し");
        wiimote.RequestIdentifyWiiMotionPlus();
        yield return new WaitForSeconds(0.5f);
        
        // ★★★★★ 修正箇所 ★★★★★
        // データレポートモードを「ボタン＋加速度＋IR(12バイト)」に設定します。
        // これによりモーションプラスが無効化され、IRデータが送信されるようになります。
        Debug.Log(">> SendDataReportMode(REPORT_BUTTONS_ACCEL_IR12) 呼び出し");
        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_IR12);
        // ★★★★★ 修正ここまで ★★★★★
        
        yield return new WaitForSeconds(0.5f);
    }
    else
    {
        // 通常のWiiリモコンの処理
        Debug.Log(">> 通常のWiiリモコン用のセットアップを実行");
        yield return new WaitForSeconds(0.2f);
    }
    
    // IRカメラをセットアップ (EXTENDED)
    Debug.Log(">> SetupIRCamera(EXTENDED) 呼び出し");
    bool result = wiimote.SetupIRCamera(IRDataType.EXTENDED);
    Debug.Log(">> IRカメラセットアップ(EXTENDED)結果: " + (result ? "成功" : "失敗"));
    
    if (!result)
    {
        // EXTENDED で失敗した場合は BASIC を試す
        Debug.Log(">> EXTENDED失敗、BASICモードで再試行");
        yield return new WaitForSeconds(0.3f);
        result = wiimote.SetupIRCamera(IRDataType.BASIC);
        Debug.Log(">> IRカメラセットアップ(BASIC)結果: " + (result ? "成功" : "失敗"));
    }
    
    yield return new WaitForSeconds(0.3f);
    
    // IRデータの初期確認
    float[] testPointer = wiimote.Ir.GetPointingPosition();
    Debug.Log(">> 初期IRテスト - 座標: [" + testPointer[0] + ", " + testPointer[1] + "]");
    
    // セットアップ完了フラグを立てる
    isIRSetupComplete = true;
    Debug.Log("========== IRカメラセットアップ完了 ==========");
}
}