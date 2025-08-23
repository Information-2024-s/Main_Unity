using UnityEngine;

public class Enemy_straight : MonoBehaviour
{
    public float speed = 3f; // m/s

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
