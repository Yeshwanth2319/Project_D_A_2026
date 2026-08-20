using UnityEngine;
using System.Collections;

public class PuzzleDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;
    public float moveDownAmount = 5f;
    public float moveSpeed = 2f;

    [Header("4 Puzzle Bases")]
    public PuzzleBase[] puzzleBases;

    private Vector3 startPosition;
    private bool doorOpened = false;

    private void Start()
    {
        startPosition = door.localPosition;
    }

    public void CheckAllPuzzles()
    {
        if (doorOpened)
            return;

        if (puzzleBases == null || puzzleBases.Length < 4)
        {
            Debug.LogWarning("Assign all 4 Puzzle Bases to PuzzleDoor!");
            return;
        }

        // Check all 4 bases
        foreach (PuzzleBase puzzle in puzzleBases)
        {
            if (!puzzle.IsSolved)
            {
                Debug.Log("Not all puzzles are solved yet.");
                return;
            }
        }

        // All 4 solved
        Debug.Log("ALL 4 PUZZLES SOLVED! Opening Door!");

        doorOpened = true;
        StartCoroutine(MoveDoorDown());
    }

    private IEnumerator MoveDoorDown()
    {
        Vector3 targetPosition =
            startPosition + Vector3.down * moveDownAmount;

        while (Vector3.Distance(door.localPosition, targetPosition) > 0.01f)
        {
            door.localPosition = Vector3.MoveTowards(
                door.localPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        door.localPosition = targetPosition;

        Debug.Log("DOOR OPENED!");
    }
}