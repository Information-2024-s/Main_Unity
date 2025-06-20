using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    public Transform[] propellers;
    public float rotateSpeed = 1000f;

    void Update()
    {
        foreach (var p in propellers)
        {
            p.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }
    }
}