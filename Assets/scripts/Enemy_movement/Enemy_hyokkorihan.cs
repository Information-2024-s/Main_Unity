using UnityEngine;

public class Enemy_hyokkorihan : MonoBehaviour
{
    public float speed = 3f; // m/s

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
