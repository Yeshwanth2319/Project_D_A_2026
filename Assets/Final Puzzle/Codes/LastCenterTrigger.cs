using System.Collections;
using UnityEngine;

public class LastCenterTrigger : MonoBehaviour
{
    [Header("Puzzle Bases")]
    public Transform[] puzzleBases;

    [Header("Puzzle Base Colliders")]
    public Collider[] puzzleBaseTriggers;

    [Header("Raise Settings")]
    public float waitTime = 1f;
    public float raiseHeight = 2f;
    public float raiseDuration = 1f;

    private bool activated = false;

    private Vector3[] startPositions;

    private void Start()
    {
        // Save starting positions
        startPositions = new Vector3[puzzleBases.Length];

        for (int i = 0; i < puzzleBases.Length; i++)
        {
            if (puzzleBases[i] != null)
            {
                startPositions[i] = puzzleBases[i].position;
            }
        }

        // Disable interaction colliders
        if (puzzleBaseTriggers != null)
        {
            foreach (Collider col in puzzleBaseTriggers)
            {
                if (col != null)
                    col.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag("Player"))
            return;

        activated = true;

        Debug.Log("CENTER TRIGGER ACTIVATED!");

        StartCoroutine(RaisePuzzleBases());
    }

    private IEnumerator RaisePuzzleBases()
    {
        Debug.Log("Waiting before raising...");

        yield return new WaitForSeconds(waitTime);

        Debug.Log("RAISING PUZZLE BASES!");

        float elapsed = 0f;

        while (elapsed < raiseDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / raiseDuration);

            // Smooth movement
            t = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < puzzleBases.Length; i++)
            {
                if (puzzleBases[i] == null)
                    continue;

                Vector3 targetPosition =
                    startPositions[i] + Vector3.up * raiseHeight;

                puzzleBases[i].position = Vector3.Lerp(
                    startPositions[i],
                    targetPosition,
                    t
                );
            }

            yield return null;
        }

        // Force final position
        for (int i = 0; i < puzzleBases.Length; i++)
        {
            if (puzzleBases[i] == null)
                continue;

            puzzleBases[i].position =
                startPositions[i] + Vector3.up * raiseHeight;
        }

        Debug.Log("PUZZLE BASES FINISHED RAISING!");

        // Enable interaction after bases are raised
        EnablePuzzleTriggers();
    }

    private void EnablePuzzleTriggers()
    {
        if (puzzleBaseTriggers == null)
            return;

        foreach (Collider col in puzzleBaseTriggers)
        {
            if (col != null)
                col.enabled = true;
        }

        Debug.Log("PUZZLE BASE INTERACTIONS ENABLED!");
    }
}