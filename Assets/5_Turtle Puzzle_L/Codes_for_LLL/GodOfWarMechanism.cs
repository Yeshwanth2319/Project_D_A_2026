using UnityEngine;

public class GodOfWarMechanism : MonoBehaviour
{
    [Header("Mechanism")]
    public Transform blueOuterRound;
    public Transform pinkInnerRound;

    [Header("Rotation")]
    public float rotationSpeed = 80f;
    public float requiredRotation = 720f;

    [Header("Pink Round")]
    public float maxRaiseAmount = 3f;

    [Header("Player")]
    public PlayerMovement playerMovement;
    public Animator playerAnimator;

    [Header("Push Movement")]
    public float pushMoveSpeed = 1.5f;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode pushKey = KeyCode.W;

    private bool playerInside = false;
    private bool puzzleSolved = false;

    private Vector3 pinkStartPosition;
    private float rotationProgress = 0f;

    private bool isPushing = false;

    private Transform playerTransform;

    private void Start()
    {
        if (pinkInnerRound != null)
        {
            pinkStartPosition = pinkInnerRound.position;
        }
    }

    private void Update()
    {
        if (puzzleSolved)
            return;

        if (!playerInside)
        {
            StopPushAnimation();
            return;
        }

        // Get player transform
        if (playerTransform == null && playerMovement != null)
        {
            playerTransform = playerMovement.transform;
        }

        // Hold E
        bool holdingE = Input.GetKey(interactKey);

        // Hold W
        bool holdingW = Input.GetKey(pushKey);

        // E + W together
        bool pushing = holdingE && holdingW;

        if (pushing)
        {
            StartPushAnimation();

            // Move player forward
            MovePlayerForward();

            // Rotate mechanism
            RotateMechanism();
        }
        else
        {
            StopPushAnimation();
        }
    }

    // =========================================
    // PLAYER PUSH MOVEMENT
    // =========================================

    private void MovePlayerForward()
    {
        if (playerTransform == null)
            return;

        Vector3 movement =
            playerTransform.forward *
            pushMoveSpeed *
            Time.deltaTime;

        playerTransform.position += movement;
    }

    // =========================================
    // PUSH ANIMATION
    // =========================================

    private void StartPushAnimation()
    {
        if (isPushing)
            return;

        isPushing = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsPushing", true);
        }
    }

    private void StopPushAnimation()
    {
        if (!isPushing)
            return;

        isPushing = false;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsPushing", false);
        }
    }

    // =========================================
    // ROTATE MECHANISM
    // =========================================

    private void RotateMechanism()
    {
        float rotationAmount =
            -rotationSpeed * Time.deltaTime;

        blueOuterRound.Rotate(
            0f,
            rotationAmount,
            0f,
            Space.World
        );

        rotationProgress += Mathf.Abs(rotationAmount);

        float progress =
            Mathf.Clamp01(
                rotationProgress / requiredRotation
            );

        Vector3 newPosition =
            pinkStartPosition +
            Vector3.up *
            (maxRaiseAmount * progress);

        pinkInnerRound.position = newPosition;

        if (progress >= 1f)
        {
            SolvePuzzle();
        }
    }

    // =========================================
    // PLAYER ENTER / EXIT
    // =========================================

    public void SetPlayerInside(bool value)
    {
        playerInside = value;

        if (value)
        {
            if (playerMovement != null)
            {
                playerTransform = playerMovement.transform;
            }
        }
        else
        {
            ReleasePlayer();
        }
    }

    private void ReleasePlayer()
    {
        StopPushAnimation();
    }

    // =========================================
    // PUZZLE SOLVED
    // =========================================

    private void SolvePuzzle()
    {
        puzzleSolved = true;

        ReleasePlayer();

        Debug.Log("🔥 GOD OF WAR MECHANISM SOLVED!");
    }
}