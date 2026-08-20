using UnityEngine;

public class MenuCharacterRotate : MonoBehaviour
{
    public float rotationSpeed = 5f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World);
        }
    }
}