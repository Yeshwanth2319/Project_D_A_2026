using UnityEngine;

public class MoveBox : MonoBehaviour
{
    public Transform endPoint;
    public float moveSpeed = 2f;

    private bool isMoving = true;

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPoint.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isMoving = false; // Stop moving when touching any trigger collider
    }
    private void OnTriggerExit(Collider other)
    {
        isMoving = true; // Stop moving when touching any trigger collider
    }
}