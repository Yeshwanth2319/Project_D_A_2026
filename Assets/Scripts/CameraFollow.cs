using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float mouseSensitivity = 200f;

    public Vector3 offset =
        new Vector3(0, 2, -5);

    float pitch = 20f;
    float yaw = 0f;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") *
               mouseSensitivity *
               Time.deltaTime;

        pitch -= Input.GetAxis("Mouse Y") *
                 mouseSensitivity *
                 Time.deltaTime;

        pitch = Mathf.Clamp(
            pitch,
            -30f,
            60f
        );

        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0
            );

        transform.position =
            player.position +
            rotation * offset;

        transform.LookAt(
            player.position +
            Vector3.up * 1.5f
        );
    }
}