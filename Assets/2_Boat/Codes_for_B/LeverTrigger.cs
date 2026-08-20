using UnityEngine;
using System.Collections;

public class LeverTrigger : MonoBehaviour
{
    [Header("Lever")]
    public Animator leverAnimator;

    [Header("Water Puzzle")]
    public int triggerID = 1;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    public string animationName = "Lever";
    public float animationSpeed = 1f;

    private bool playerInside = false;
    private bool isOn = false;
    private bool isAnimating = false;

    private Coroutine animationCoroutine;

    private void Start()
    {
        if (leverAnimator == null)
        {
            Debug.LogError(gameObject.name + ": Lever Animator is not assigned!");
            return;
        }

        // Stop Animator
        leverAnimator.speed = 0f;

        // Put lever at beginning
        leverAnimator.Play(animationName, 0, 0f);
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (isAnimating)
                return;

            // If puzzle is already solved, don't allow lever
            if (WaterPuzzleManager.Instance != null &&
                WaterPuzzleManager.Instance.IsPuzzleSolved())
            {
                return;
            }

            if (isOn)
            {
                TurnOffLever();
            }
            else
            {
                TurnOnLever();
            }
        }
    }

    private void TurnOnLever()
    {
        isOn = true;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(
            PlayLeverAnimation(0f, 1f)
        );

        // Tell puzzle manager
        if (WaterPuzzleManager.Instance != null)
        {
            WaterPuzzleManager.Instance.ActivateTrigger(triggerID);
        }
    }

    private void TurnOffLever()
    {
        isOn = false;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(
            PlayLeverAnimation(1f, 0f)
        );
    }

    private IEnumerator PlayLeverAnimation(float startTime, float endTime)
    {
        isAnimating = true;

        leverAnimator.speed = animationSpeed;

        float currentTime = startTime;

        // Forward
        if (endTime > startTime)
        {
            leverAnimator.Play(animationName, 0, startTime);

            while (currentTime < endTime)
            {
                currentTime += Time.deltaTime * animationSpeed;

                leverAnimator.Play(
                    animationName,
                    0,
                    Mathf.Clamp01(currentTime)
                );

                yield return null;
            }
        }
        // Backward
        else
        {
            leverAnimator.speed = 0f;

            while (currentTime > endTime)
            {
                currentTime -= Time.deltaTime * animationSpeed;

                leverAnimator.Play(
                    animationName,
                    0,
                    Mathf.Clamp01(currentTime)
                );

                yield return null;
            }
        }

        leverAnimator.speed = 0f;

        leverAnimator.Play(
            animationName,
            0,
            endTime
        );

        isAnimating = false;
        animationCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
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