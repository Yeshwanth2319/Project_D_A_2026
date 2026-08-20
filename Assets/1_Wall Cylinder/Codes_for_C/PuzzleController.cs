using UnityEngine;
using System.Collections;

public class PuzzleController : MonoBehaviour
{
    [Header("Cylinders")]
    public PuzzleCylinder[] cylinders;

    [Header("Selection")]
    public Transform selectionArrow;
    public Transform[] arrowPoints;

    private int selectedCylinder = 0;

    [Header("Puzzle State")]
    public bool inPuzzle;

    private bool solved = false;
    private bool puzzleCompleted = false;

    [Header("Player")]
    public MonoBehaviour playerMovement;

    [Header("Wall")]
    public Transform wall;

    // How far the wall moves downward
    public float wallDownDistance = 3f;

    // How fast the wall moves
    public float wallMoveSpeed = 2f;

    private Vector3 wallStartPosition;
    private Vector3 wallTargetPosition;

    private void Start()
    {
        if (selectionArrow != null)
            selectionArrow.gameObject.SetActive(false);

        // Save wall starting position
        if (wall != null)
        {
            wallStartPosition = wall.position;

            wallTargetPosition = wallStartPosition +
                                 Vector3.down * wallDownDistance;
        }
    }

    private void Update()
    {
        if (!inPuzzle)
            return;

        UpdateArrow();

        // A = Left
        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedCylinder--;

            if (selectedCylinder < 0)
                selectedCylinder = 0;
        }

        // D = Right
        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedCylinder++;

            if (selectedCylinder >= cylinders.Length)
                selectedCylinder = cylinders.Length - 1;
        }

        // E = Rotate
        if (Input.GetKeyDown(KeyCode.E))
        {
            cylinders[selectedCylinder].RotateCylinder();
        }

        // F = Lock / Unlock
        if (Input.GetKeyDown(KeyCode.F))
        {
            cylinders[selectedCylinder].ToggleLock();

            CheckPuzzle();
        }

        // ESC = Exit Puzzle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPuzzle();
        }
    }

    private void UpdateArrow()
    {
        if (selectionArrow == null)
            return;

        if (arrowPoints == null || arrowPoints.Length == 0)
            return;

        selectionArrow.position =
            arrowPoints[selectedCylinder].position;

        selectionArrow.rotation =
            arrowPoints[selectedCylinder].rotation;
    }

    public void EnterPuzzle()
    {
        if (puzzleCompleted)
            return;

        inPuzzle = true;

        selectedCylinder = 0;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (selectionArrow != null)
            selectionArrow.gameObject.SetActive(true);

        UpdateArrow();

        Debug.Log("Puzzle Started");
    }

    private void ExitPuzzle()
    {
        inPuzzle = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (selectionArrow != null)
            selectionArrow.gameObject.SetActive(false);

        Debug.Log("Puzzle Closed");
    }

    private void CheckPuzzle()
    {
        // All cylinders must be locked
        foreach (PuzzleCylinder cylinder in cylinders)
        {
            if (!cylinder.locked)
                return;
        }

        // All cylinders must be Fish
        foreach (PuzzleCylinder cylinder in cylinders)
        {
            if (cylinder.currentState != 0)
            {
                Debug.Log("Wrong Solution");
                return;
            }
        }

        // Puzzle solved
        if (!solved)
        {
            solved = true;

            Debug.Log("Puzzle Solved");

            StartCoroutine(SolvePuzzle());
        }
    }

    private IEnumerator SolvePuzzle()
    {
        puzzleCompleted = true;

        // Small delay before wall starts moving
        yield return new WaitForSeconds(0.5f);

        // Move wall down
        if (wall != null)
        {
            while (Vector3.Distance(wall.position, wallTargetPosition) > 0.01f)
            {
                wall.position = Vector3.MoveTowards(
                    wall.position,
                    wallTargetPosition,
                    wallMoveSpeed * Time.deltaTime
                );

                yield return null;
            }

            // Make sure it reaches the exact position
            wall.position = wallTargetPosition;
        }

        yield return new WaitForSeconds(0.5f);

        ExitPuzzle();
    }
}