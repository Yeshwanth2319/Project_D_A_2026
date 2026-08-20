using UnityEngine;
using System.Collections;

public class PuzzleWheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public float rotationAmount = 90f;
    public float rotationSpeed = 5f;

    [Header("Solution")]
    public int correctAngle = 0;

    [Header("Selection Glow")]
    public GameObject selectionGlow;

    private int currentAngle = 0;
    private bool isRotating = false;

    public bool IsCorrect
    {
        get
        {
            return currentAngle == correctAngle;
        }
    }

    private void Start()
    {
        if (selectionGlow != null)
            selectionGlow.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionGlow != null)
            selectionGlow.SetActive(selected);
    }

    public void RotateWheel()
    {
        if (isRotating)
            return;

        currentAngle += 90;

        if (currentAngle >= 360)
            currentAngle -= 360;

        StartCoroutine(RotateSmoothly());
    }

    private IEnumerator RotateSmoothly()
    {
        isRotating = true;

        Quaternion startRotation = transform.localRotation;

        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, rotationAmount, 0f);

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * rotationSpeed;

            transform.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                time
            );

            yield return null;
        }

        transform.localRotation = targetRotation;

        isRotating = false;
    }
}