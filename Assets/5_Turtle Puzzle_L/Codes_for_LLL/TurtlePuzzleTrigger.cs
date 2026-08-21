using UnityEngine;
using System.Collections;

public class TurtlePuzzleTrigger : MonoBehaviour
{
    [Header("Turtles")]
    public Transform[] turtles;

    [Header("Raise Settings")]
    public float raiseAmount = 1f;
    public float raiseSpeed = 2f;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;
    private bool puzzleStarted = false;

    private Vector3[] startPositions;

    private void Start()
    {
        // Store original underground positions
        startPositions = new Vector3[turtles.Length];

        for (int i = 0; i < turtles.Length; i++)
        {
            startPositions[i] = turtles[i].position;
        }
    }

    private void Update()
    {
        if (playerInside && !puzzleStarted && Input.GetKeyDown(interactKey))
        {
            puzzleStarted = true;

            StartCoroutine(RaiseTurtles());
        }
    }

    private IEnumerator RaiseTurtles()
    {
        Vector3[] targetPositions = new Vector3[turtles.Length];

        // Calculate target positions
        for (int i = 0; i < turtles.Length; i++)
        {
            targetPositions[i] = startPositions[i] + Vector3.up * raiseAmount;
        }

        bool moving = true;

        while (moving)
        {
            moving = false;

            for (int i = 0; i < turtles.Length; i++)
            {
                turtles[i].position = Vector3.MoveTowards(
                    turtles[i].position,
                    targetPositions[i],
                    raiseSpeed * Time.deltaTime
                );

                if (Vector3.Distance(turtles[i].position, targetPositions[i]) > 0.01f)
                {
                    moving = true;
                }
            }

            yield return null;
        }
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