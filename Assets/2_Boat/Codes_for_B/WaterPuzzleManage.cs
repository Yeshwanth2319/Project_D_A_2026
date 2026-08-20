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

    // Correct sequence:
    // 1 -> 3 -> 4 -> 2
    private readonly int[] correctSequence = { 1, 3, 4, 2 };

    private int puzzleStep = 0;

    private bool puzzleSolved = false;

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
        // Save original wall positions
        wall1Start = wall1.position;
        wall2Start = wall2.position;
        wall3Start = wall3.position;
        wall4Start = wall4.position;

        puzzleStep = 0;
        puzzleSolved = false;

        Debug.Log("Water Puzzle Started.");
        Debug.Log("Correct Sequence: 1 -> 3 -> 4 -> 2");
    }

    // =========================================================
    // TRIGGER ACTIVATION
    // =========================================================

    public bool ActivateTrigger(int triggerID)
    {
        // Puzzle already solved
        if (puzzleSolved)
        {
            Debug.Log("Puzzle already solved. Trigger " + triggerID + " ignored.");
            return false;
        }

        Debug.Log(
            "Trigger " + triggerID +
            " activated. Current puzzle step: " + puzzleStep
        );

        // Safety check
        if (puzzleStep < 0 || puzzleStep >= correctSequence.Length)
        {
            puzzleStep = 0;
        }

        int expectedTrigger = correctSequence[puzzleStep];

        // =====================================================
        // CORRECT TRIGGER
        // =====================================================

        if (triggerID == expectedTrigger)
        {
            puzzleStep++;

            Debug.Log(
                "CORRECT! Trigger " + triggerID +
                " activated."
            );

            // STEP 1
            if (triggerID == 1)
            {
                MoveDown(wall1, wall1Start);
                MoveDown(wall3, wall3Start);

                Debug.Log("Step 1 complete.");
            }

            // STEP 2
            else if (triggerID == 3)
            {
                MoveUp(wall1, wall1Start);
                MoveUp(wall2, wall2Start);
                MoveDown(wall4, wall4Start);

                Debug.Log("Step 2 complete.");
            }

            // STEP 3
            else if (triggerID == 4)
            {
                MoveUp(wall4, wall4Start);
                MoveDown(wall2, wall2Start);
                MoveDown(wall3, wall3Start);

                Debug.Log("Step 3 complete.");
            }

            // STEP 4
            else if (triggerID == 2)
            {
                SolvePuzzle();
            }

            return true;
        }

        // =====================================================
        // WRONG TRIGGER
        // =====================================================

        Debug.Log(
            "WRONG TRIGGER! Expected: " +
            expectedTrigger +
            " but received: " +
            triggerID
        );

        ResetPuzzle();

        return false;
    }

    // =========================================================
    // SOLVE PUZZLE
    // =========================================================

    private void SolvePuzzle()
    {
        puzzleSolved = true;

        Debug.Log("=================================");
        Debug.Log("PUZZLE SOLVED!");
        Debug.Log("=================================");

        // Final wall positions
        MoveDown(wall1, wall1Start);
        MoveDown(wall2, wall2Start);
        MoveDown(wall3, wall3Start);
        MoveDown(wall4, wall4Start);

        // Disable triggers
        DisablePuzzleTriggers();
    }

    // =========================================================
    // RESET PUZZLE
    // =========================================================

    private void ResetPuzzle()
    {
        if (puzzleSolved)
            return;

        Debug.Log("Wrong Order - Reset");

        puzzleStep = 0;

        // Return every wall to starting position
        MoveUp(wall1, wall1Start);
        MoveUp(wall2, wall2Start);
        MoveUp(wall3, wall3Start);
        MoveUp(wall4, wall4Start);

        Debug.Log("Puzzle reset. Start again with Trigger 1.");
    }

    // =========================================================
    // DISABLE TRIGGERS
    // =========================================================

    private void DisablePuzzleTriggers()
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

    // =========================================================
    // MOVE DOWN
    // =========================================================

    private void MoveDown(Transform wall, Vector3 startPos)
    {
        if (wall == null)
            return;

        float targetY = startPos.y - loweredAmount;

        StartWallMovement(wall, targetY);
    }

    // =========================================================
    // MOVE UP
    // =========================================================

    private void MoveUp(Transform wall, Vector3 startPos)
    {
        if (wall == null)
            return;

        float targetY = startPos.y;

        StartWallMovement(wall, targetY);
    }

    // =========================================================
    // START WALL MOVEMENT
    // =========================================================

    private void StartWallMovement(Transform wall, float targetY)
    {
        if (wall == null)
            return;

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

    // =========================================================
    // WALL MOVEMENT COROUTINE
    // =========================================================

    private IEnumerator MoveWall(Transform wall, float targetY)
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

    // =========================================================
    // PUBLIC STATUS
    // =========================================================

    public bool IsPuzzleSolved()
    {
        return puzzleSolved;
    }

    public int GetPuzzleStep()
    {
        return puzzleStep;
    }
}