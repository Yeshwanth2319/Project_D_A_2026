using UnityEngine;
using System.Collections;

public class PuzzleBase : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;
    public Camera puzzleCamera;

    [Header("Puzzle Camera Point")]
    public Transform puzzleCameraPoint;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode exitKey = KeyCode.Escape;

    [Header("Camera Movement")]
    public float cameraMoveSpeed = 3f;
    public float cameraRotateSpeed = 5f;

    [Header("Wheels")]
    public PuzzleWheel[] wheels;

    [Header("Door")]
    public PuzzleDoor puzzleDoor;

    [Header("TEST")]
    public bool testSolved = false;

    private int selectedWheel = 0;

    private bool playerInside = false;
    private bool puzzleOpen = false;
    private bool puzzleSolved = false;

    public bool IsSolved
    {
        get { return puzzleSolved; }
    }

    private Transform player;
    private MonoBehaviour playerMovement;

    private void Start()
    {
        puzzleCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        // TEST SOLVED
        if (testSolved && !puzzleSolved)
        {
            PuzzleSolved();
        }

        // ENTER PUZZLE
        if (playerInside && !puzzleOpen && !puzzleSolved && Input.GetKeyDown(interactKey))
        {
            StartPuzzle();
        }

        // PUZZLE CONTROLS
        if (puzzleOpen)
        {
            // E = Select next wheel
            if (Input.GetKeyDown(KeyCode.E))
            {
                SelectNextWheel();
            }

            // R = Rotate selected wheel
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateSelectedWheel();
            }

            // ESC = Exit puzzle
            if (Input.GetKeyDown(exitKey))
            {
                ExitPuzzle();
            }
        }
    }

    private void SelectNextWheel()
    {
        if (wheels == null || wheels.Length == 0)
            return;

        selectedWheel++;

        if (selectedWheel >= wheels.Length)
            selectedWheel = 0;

        UpdateWheelSelection();

        Debug.Log("Selected Wheel: " + selectedWheel);
    }

    private void RotateSelectedWheel()
    {
        if (wheels == null || wheels.Length == 0)
            return;

        wheels[selectedWheel].RotateWheel();

        CheckPuzzleSolved();
    }

    private void CheckPuzzleSolved()
    {
        foreach (PuzzleWheel wheel in wheels)
        {
            if (!wheel.IsCorrect)
                return;
        }

        PuzzleSolved();
    }

    private void PuzzleSolved()
    {
        if (puzzleSolved)
            return;

        Debug.Log("PUZZLE SOLVED!");

        puzzleSolved = true;
        puzzleOpen = false;

        // Stop camera coroutine
        StopAllCoroutines();

        // Disable puzzle camera
        if (puzzleCamera != null)
            puzzleCamera.gameObject.SetActive(false);

        // Enable player camera
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        // Enable player movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Remove wheel selection glow
        if (wheels != null)
        {
            foreach (PuzzleWheel wheel in wheels)
            {
                wheel.SetSelected(false);
            }
        }

        // Tell door to check all puzzle bases
        if (puzzleDoor != null)
        {
            puzzleDoor.CheckAllPuzzles();
        }

        Debug.Log("Base Puzzle Completed!");
    }

    private void StartPuzzle()
    {
        puzzleOpen = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        puzzleCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        selectedWheel = 0;

        UpdateWheelSelection();

        StartCoroutine(MovePuzzleCamera());
    }

    private void UpdateWheelSelection()
    {
        if (wheels == null || wheels.Length == 0)
            return;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].SetSelected(i == selectedWheel);
        }
    }

    private IEnumerator MovePuzzleCamera()
    {
        Transform cam = puzzleCamera.transform;

        Vector3 startPosition = cam.position;
        Quaternion startRotation = cam.rotation;

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * cameraMoveSpeed;

            cam.position = Vector3.Lerp(
                startPosition,
                puzzleCameraPoint.position,
                time
            );

            cam.rotation = Quaternion.Slerp(
                startRotation,
                puzzleCameraPoint.rotation,
                time
            );

            yield return null;
        }

        cam.position = puzzleCameraPoint.position;
        cam.rotation = puzzleCameraPoint.rotation;

        Debug.Log("Puzzle camera ready!");
    }

    private void ExitPuzzle()
    {
        puzzleOpen = false;

        StopAllCoroutines();

        if (puzzleCamera != null)
            puzzleCamera.gameObject.SetActive(false);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        Debug.Log("Exited puzzle!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            player = other.transform;

            playerMovement = player.GetComponent<MonoBehaviour>();
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