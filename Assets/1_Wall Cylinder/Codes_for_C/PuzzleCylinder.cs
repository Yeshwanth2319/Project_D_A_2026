using UnityEngine;
using System.Collections;

public class PuzzleCylinder : MonoBehaviour
{
    public int currentState = 0; // 0 Fish, 1 Octopus, 2 Shell

    public bool locked = false;

    public float rotateDuration = 0.3f;

    private bool isRotating = false;

    public void RotateCylinder()
    {
        if (locked || isRotating)
            return;

        currentState++;

        if (currentState > 2)
            currentState = 0;

        StartCoroutine(RotateSmooth());
    }

    IEnumerator RotateSmooth()
    {
        isRotating = true;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(120f, 0f, 0f);

        float time = 0f;

        while (time < rotateDuration)
        {
            transform.rotation = Quaternion.Slerp(
                startRot,
                targetRot,
                time / rotateDuration);

            time += Time.deltaTime;

            yield return null;
        }

        transform.rotation = targetRot;

        isRotating = false;
    }

    public void ToggleLock()
    {
        if (isRotating)
            return;

        locked = !locked;

        Debug.Log(gameObject.name +
            (locked ? " LOCKED" : " UNLOCKED"));
    }
}