using UnityEngine;
using System.Collections;

public class TurtlePuzzleManager : MonoBehaviour
{
    public static TurtlePuzzleManager Instance;

    [Header("Turtles")]
    public int totalTurtles = 6;

    [Header("Solved Object")]
    public Transform smallObject;

    [Header("Raise Settings")]
    public float raiseAmount = 2f;
    public float raiseSpeed = 2f;

    [Header("TEST")]
    public KeyCode solveTestKey = KeyCode.T;

    private int turtlesSolved = 0;
    private bool puzzleSolved = false;

    private Vector3 smallObjectStartPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (smallObject != null)
        {
            smallObjectStartPosition = smallObject.position;
        }
    }

    private void Update()
    {
        // TEST ONLY
        if (Input.GetKeyDown(solveTestKey))
        {
            SolvePuzzle();
        }
    }

    public void TurtleRotated()
    {
        turtlesSolved++;

        Debug.Log("Turtles Solved: " + turtlesSolved + " / " + totalTurtles);

        if (turtlesSolved >= totalTurtles)
        {
            SolvePuzzle();
        }
    }

    public void SolvePuzzle()
    {
        if (puzzleSolved)
            return;

        puzzleSolved = true;

        Debug.Log("🐢 TURTLE PUZZLE SOLVED!");

        StartCoroutine(RaiseSmallObject());
    }

    private IEnumerator RaiseSmallObject()
    {
        if (smallObject == null)
        {
            Debug.LogWarning("Small Object is not assigned!");
            yield break;
        }

        Vector3 targetPosition =
            smallObjectStartPosition + Vector3.up * raiseAmount;

        while (Vector3.Distance(
            smallObject.position,
            targetPosition) > 0.01f)
        {
            smallObject.position = Vector3.MoveTowards(
                smallObject.position,
                targetPosition,
                raiseSpeed * Time.deltaTime
            );

            yield return null;
        }

        smallObject.position = targetPosition;

        Debug.Log("🐢 Small Object Raised!");
    }
}