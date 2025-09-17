using UnityEngine;

public class CircleGroupController : MonoBehaviour
{
    public float rotationSpeed;
    public bool clockwise;
    public Vector3 rotationAxis = Vector3.forward; // スポナーによって設定される

    void Update()
    {
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(rotationAxis, rotationSpeed * direction * Time.deltaTime);
    }
}
