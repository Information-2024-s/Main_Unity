using UnityEngine;

public class Syouzyun : MonoBehaviour
{
    [SerializeField] public GameObject bullet_nomal;
    [SerializeField] public GameObject bullet_sniper;
    [SerializeField] public GameObject bullet_machinegun;
    [SerializeField] private float power = 500f;
    public float cooltime = 0.2f;

    private float timer = 0f;

    public AudioClip sound1;

    public int bullettype = 0;
    int count = 0;
    private int overheat = 20;              // オーバーヒート発射数の上限
    private float countReduceInterval = 1f;   // カウント減少の間隔（秒）
    private float countReduceTimer = 0f;      // 減少タイマー
    

    void Start()
    {
        if (bullettype == 0)
        {
            cooltime = 1f;
            power = 100f;
        }
        else if (bullettype == 1)
        {
            cooltime = 1.5f;
            power = 1000f;
        }
        else if (bullettype == 2)
        {
            cooltime = 0.05f;
            overheat = 20;
            power = 100f;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        GameObject bulletObj = null;

        if (bullettype == 2 && count > 0)
        {
            countReduceTimer += Time.deltaTime;
            if (countReduceTimer >= countReduceInterval)
            {
                countReduceTimer = 0f;
                count -= 5;
            }
        }

        if (Input.GetButton("Fire1") && timer >= cooltime)
        {
            // --- ここから追加 ---
            // LastBossがフェード中の場合は、ここで処理を中断して射撃しない
            if (LastBoss.Instance != null && LastBoss.Instance.IsFading)
            {
                return;
            }
            // --- ここまで追加 ---

            timer = 0f;
            count++;

            if (count >= overheat && bullettype == 2)
            {
                count = overheat;
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (bullettype == 0)
                bulletObj = Instantiate(bullet_nomal, transform.position, Quaternion.identity);
            else if (bullettype == 1)
                bulletObj = Instantiate(bullet_sniper, transform.position, Quaternion.identity);
            else if (bullettype == 2)
                bulletObj = Instantiate(bullet_machinegun, transform.position, Quaternion.identity);

            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
            AudioSource.PlayClipAtPoint(sound1, transform.position, 1.0f);

            if (rb != null)
            {
                rb.AddForce(ray.direction * power, ForceMode.Impulse);
            }
        }
    }
}