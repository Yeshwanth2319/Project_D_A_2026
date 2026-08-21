using UnityEngine;
using System.Collections;

public class TurtleRotate : MonoBehaviour
{
    [Header("Center")]
    public Transform centerPoint;

    [Header("Rotation")]
    public float rotationSpeed = 180f;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;
    private bool rotated = false;
    private bool rotating = false;

    private void Update()
    {
        if (playerInside && !rotated && !rotating)
        {
            if (Input.GetKeyDown(interactKey))
            {
                StartCoroutine(RotateToCenter());
            }
        }
    }

    private IEnumerator RotateToCenter()
    {
        rotating = true;

        // Direction from turtle to center
        Vector3 direction = centerPoint.position - transform.position;

        // Ignore vertical difference
        direction.y = 0f;

        // Calculate rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = targetRotation;

        rotated = true;
        rotating = false;

        // Tell puzzle manager this turtle is solved
        TurtlePuzzleManager.Instance.TurtleRotated();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}