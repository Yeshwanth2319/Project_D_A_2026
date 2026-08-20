using UnityEngine;
using System.Collections;

public class WaterPuzzleManager : MonoBehaviour
{
    public static WaterPuzzleManager Instance;

    [Header("Walls")]
    public Transform wall1;
    public Transform wall2;
    public Transform wall3;
    public Transform wall4;

    [Header("Trigger Objects")]
    public GameObject[] puzzleTriggers;

    [Header("Settings")]
    public float loweredAmount = 4f;
    public float moveSpeed = 2f;

    private Vector3 wall1Start;
    private Vector3 wall2Start;
    private Vector3 wall3Start;
    private Vector3 wall4Start;

    private int puzzleStep = 0;

    // Puzzle lock
    private bool puzzleSolved = false;

    // One movement coroutine for each wall
    private Coroutine wall1Coroutine;
    private Coroutine wall2Coroutine;
    private Coroutine wall3Coroutine;
    private Coroutine wall4Coroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        wall1Start = wall1.position;
        wall2Start = wall2.position;
        wall3Start = wall3.position;
        wall4Start = wall4.position;
    }

    public void ActivateTrigger(int triggerID)
    {
        // Completely ignore trigger input after puzzle is solved
        if (puzzleSolved)
        {
            Debug.Log("Puzzle already solved. Trigger disabled.");
            return;
        }

        switch (triggerID)
        {
            case 1:

                if (puzzleStep == 0)
                {
                    puzzleStep = 1;

                    MoveDown(wall1, wall1Start);
                    MoveDown(wall3, wall3Start);

                    Debug.Log("Step 1 Correct");
                }
                else
                {
                    ResetPuzzle();
                }

                break;


            case 3:

                if (puzzleStep == 1)
                {
                    puzzleStep = 2;

                    MoveUp(wall1, wall1Start);
                    MoveUp(wall2, wall2Start);
                    MoveDown(wall4, wall4Start);

                    Debug.Log("Step 2 Correct");
                }
                else
                {
                    ResetPuzzle();
                }

                break;


            case 4:

                if (puzzleStep == 2)
                {
                    puzzleStep = 3;

                    MoveUp(wall4, wall4Start);
                    MoveDown(wall2, wall2Start);
                    MoveDown(wall3, wall3Start);

                    Debug.Log("Step 3 Correct");
                }
                else
                {
                    ResetPuzzle();
                }

                break;


            case 2:

                if (puzzleStep == 3)
                {
                    SolvePuzzle();
                }
                else
                {
                    ResetPuzzle();
                }

                break;
        }
    }

    void SolvePuzzle()
    {
        // Lock puzzle permanently
        puzzleSolved = true;

        Debug.Log("PUZZLE SOLVED!");

        // Final wall positions
        MoveDown(wall1, wall1Start);
        MoveDown(wall2, wall2Start);
        MoveDown(wall3, wall3Start);
        MoveDown(wall4, wall4Start);

        // Disable all puzzle triggers
        DisablePuzzleTriggers();
    }

    void ResetPuzzle()
    {
        // Don't allow reset after solving
        if (puzzleSolved)
            return;

        Debug.Log("Wrong Order - Reset");

        puzzleStep = 0;

        MoveUp(wall1, wall1Start);
        MoveUp(wall2, wall2Start);
        MoveUp(wall3, wall3Start);
        MoveUp(wall4, wall4Start);
    }

    void DisablePuzzleTriggers()
    {
        if (puzzleTriggers == null)
            return;

        foreach (GameObject trigger in puzzleTriggers)
        {
            if (trigger == null)
                continue;

            Collider col = trigger.GetComponent<Collider>();

            if (col != null)
            {
                col.enabled = false;
            }
        }

        Debug.Log("All puzzle triggers disabled.");
    }

    void MoveDown(Transform wall, Vector3 startPos)
    {
        float targetY = startPos.y - loweredAmount;

        StartWallMovement(wall, targetY);
    }

    void MoveUp(Transform wall, Vector3 startPos)
    {
        float targetY = startPos.y;

        StartWallMovement(wall, targetY);
    }

    void StartWallMovement(Transform wall, float targetY)
    {
        if (wall == wall1)
        {
            if (wall1Coroutine != null)
                StopCoroutine(wall1Coroutine);

            wall1Coroutine = StartCoroutine(
                MoveWall(wall, targetY)
            );
        }
        else if (wall == wall2)
        {
            if (wall2Coroutine != null)
                StopCoroutine(wall2Coroutine);

            wall2Coroutine = StartCoroutine(
                MoveWall(wall, targetY)
            );
        }
        else if (wall == wall3)
        {
            if (wall3Coroutine != null)
                StopCoroutine(wall3Coroutine);

            wall3Coroutine = StartCoroutine(
                MoveWall(wall, targetY)
            );
        }
        else if (wall == wall4)
        {
            if (wall4Coroutine != null)
                StopCoroutine(wall4Coroutine);

            wall4Coroutine = StartCoroutine(
                MoveWall(wall, targetY)
            );
        }
    }

    IEnumerator MoveWall(Transform wall, float targetY)
    {
        Vector3 targetPos = new Vector3(
            wall.position.x,
            targetY,
            wall.position.z
        );

        while (Vector3.Distance(wall.position, targetPos) > 0.01f)
        {
            wall.position = Vector3.MoveTowards(
                wall.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        wall.position = targetPos;
    }

    public bool IsPuzzleSolved()
    {
        return puzzleSolved;
    }
}