using UnityEngine;

public class MirrorPush : MonoBehaviour
{
    [HideInInspector]
    public bool playerInside;

    [HideInInspector]
    public Transform player;

    public float pushSpeed = 2f;

    private Rigidbody rb;
    private Animator playerAnimator;

    private bool isPushing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = 100f;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!playerInside || player == null)
        {
            StopPushAnimation();
            return;
        }

        if (playerAnimator == null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }

        // Check input immediately
        bool pushInput =
            Input.GetKey(KeyCode.E) &&
            Input.GetKey(KeyCode.W);

        if (pushInput && !isPushing)
        {
            StartPushAnimation();
        }
        else if (!pushInput && isPushing)
        {
            StopPushAnimation();
        }
    }

    void FixedUpdate()
    {
        if (!playerInside || player == null)
            return;

        if (Input.GetKey(KeyCode.E) &&
            Input.GetKey(KeyCode.W))
        {
            Vector3 dir = player.forward;

            rb.MovePosition(
                rb.position +
                dir * pushSpeed *
                Time.fixedDeltaTime
            );
        }
    }

    void StartPushAnimation()
    {
        isPushing = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsPushing", true);
        }
    }

    void StopPushAnimation()
    {
        if (!isPushing)
            return;

        isPushing = false;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsPushing", false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            playerInside = false;
            StopPushAnimation();
        }
    }
}