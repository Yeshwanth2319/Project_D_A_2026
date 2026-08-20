using UnityEngine;

public class PuzzleTrigger_2 : MonoBehaviour
{
    public PuzzleController puzzleController;

    private bool playerInside = false;

    private void Update()
    {
        if (playerInside &&
            Input.GetKeyDown(KeyCode.E) &&
            !puzzleController.inPuzzle)
        {
            Debug.Log("E Pressed - Starting Puzzle");

            puzzleController.EnterPuzzle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            Debug.Log("Player entered puzzle area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            Debug.Log("Player left puzzle area");
        }
    }
}