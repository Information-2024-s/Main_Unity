using UnityEngine;

public class Syouzyun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float power = 500f;
    public float cooltime = 0.2f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= cooltime)
        {
            timer = 0f;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            GameObject bulletObj = Instantiate(bullet, transform.position, Quaternion.identity);
            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(ray.direction * power, ForceMode.Impulse);
            }
        }
    }
}
