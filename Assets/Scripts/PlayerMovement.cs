using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    public Transform cameraTransform;
    public Transform groundCheck;


    // =========================================================
    // MOVEMENT
    // =========================================================

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;


    // =========================================================
    // CROUCH
    // =========================================================

    [Header("Crouch")]
    public KeyCode crouchKey = KeyCode.C;
    public float crouchSpeed = 2f;
    public CharControllerSize crouchSize;


    // =========================================================
    // SLIDE
    // =========================================================

    [Header("Slide")]
    public KeyCode slideKey = KeyCode.LeftControl;
    public float slideSpeed = 6f;
    public CharControllerSize slideSize;


    // =========================================================
    // FREEHANG
    // =========================================================

    [Header("Freehang")]
    public KeyCode hangKey = KeyCode.E;
    public float hangCheckDistance = 1.2f;
    public float hangHeight = 1.5f;
    public float hangForwardOffset = 0.35f;
    public LayerMask climbableMask;


    // =========================================================
    // GROUND CHECK
    // =========================================================

    [Header("Ground Check")]
    public float groundDistance = 0.2f;
    public LayerMask groundMask;


    // =========================================================
    // GUN
    // =========================================================

    [Header("Information Gun")]
    [SerializeField]
    private bool isGunOut = false;

    public bool IsGunOut
    {
        get { return isGunOut; }
    }


    // =========================================================
    // COMPONENTS
    // =========================================================

    private CharacterController controller;
    private Animator animator;


    // =========================================================
    // PLAYER STATE
    // =========================================================

    private Vector3 velocity;

    private bool isGrounded;
    private bool isSliding;
    private bool isCrouching;
    private bool isHanging;


    // =========================================================
    // HANG VARIABLES
    // =========================================================

    private Vector3 hangPosition;
    private Quaternion hangRotation;


    // =========================================================
    // ORIGINAL CHARACTER CONTROLLER SIZE
    // =========================================================

    private CharControllerSize originalSize;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (controller == null)
        {
            Debug.LogError(
                "PlayerMovement: CharacterController not found!"
            );
        }

        if (animator == null)
        {
            Debug.LogError(
                "PlayerMovement: Animator not found!"
            );
        }

        originalSize.centre =
            controller.center;

        originalSize.height =
            controller.height;

        isGunOut = false;

        animator.SetBool(
            "IsGunOut",
            false
        );
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        GroundCheck();

        HandleFreehang();

        HandleCrouchToggle();

        HandleMovement();

        HandleCrouchMovement();

        HandleJump();

        HandleSlide();

        ApplyGravity();

        UpdateCrouchAnimation();
    }


    // =========================================================
    // GROUND CHECK
    // =========================================================

    void GroundCheck()
    {
        if (isHanging)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }


    // =========================================================
    // NORMAL MOVEMENT
    // =========================================================

    void HandleMovement()
    {
        if (isSliding ||
            isCrouching ||
            isHanging)
        {
            return;
        }

        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        Vector3 inputDirection =
            new Vector3(
                horizontal,
                0f,
                vertical
            );

        inputDirection =
            Vector3.ClampMagnitude(
                inputDirection,
                1f
            );

        bool moving =
            inputDirection.magnitude > 0.1f;

        bool running =
            Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = 0f;

        if (moving)
        {
            // =================================================
            // CAMERA RELATIVE MOVEMENT
            // =================================================

            Vector3 cameraForward =
                cameraTransform.forward;

            Vector3 cameraRight =
                cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection =
                cameraForward *
                inputDirection.z +
                cameraRight *
                inputDirection.x;

            moveDirection.Normalize();


            // =================================================
            // SPEED
            // =================================================

            targetSpeed =
                running ? 1f : 0.5f;

            float movementSpeed =
                running ? runSpeed : walkSpeed;


            // =================================================
            // MOVE
            // =================================================

            controller.Move(
                moveDirection *
                movementSpeed *
                Time.deltaTime
            );


            // =================================================
            // ROTATE
            // =================================================

            transform.forward =
                Vector3.Slerp(
                    transform.forward,
                    moveDirection,
                    rotationSpeed *
                    Time.deltaTime
                );
        }


        // =====================================================
        // LOCOMOTION ANIMATION
        // =====================================================

        animator.SetFloat(
            "Speed",
            targetSpeed,
            0.1f,
            Time.deltaTime
        );
    }


    // =========================================================
    // JUMP
    // =========================================================

    void HandleJump()
    {
        // Gun blocks jump
        if (IsGunOut)
            return;

        // Crouch blocks jump
        if (isCrouching)
            return;

        // Slide blocks jump
        if (isSliding)
            return;

        // Hang blocks jump
        if (isHanging)
            return;


        if (Input.GetKeyDown(KeyCode.Space) &&
            isGrounded)
        {
            velocity.y = jumpForce;

            animator.SetBool(
                "IsJumping",
                true
            );
        }


        // =====================================================
        // LAND
        // =====================================================

        if (isGrounded &&
            velocity.y < 0f)
        {
            animator.SetBool(
                "IsJumping",
                false
            );
        }
    }


    // =========================================================
    // CROUCH TOGGLE
    // =========================================================

    void HandleCrouchToggle()
    {
        // Gun blocks crouch
        if (IsGunOut)
            return;

        // Slide blocks crouch
        if (isSliding)
            return;

        // Hang blocks crouch
        if (isHanging)
            return;


        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching =
                !isCrouching;

            animator.SetBool(
                "IsCrouching",
                isCrouching
            );


            if (isCrouching)
            {
                SetCharacterControllerSize(
                    crouchSize
                );
            }
            else
            {
                SetCharacterControllerSize(
                    originalSize
                );
            }
        }
    }


    // =========================================================
    // CROUCH MOVEMENT
    // =========================================================

    void HandleCrouchMovement()
    {
        if (!isCrouching)
            return;

        if (IsGunOut ||
            isHanging)
        {
            return;
        }

        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        Vector3 inputDirection =
            new Vector3(
                horizontal,
                0f,
                vertical
            );

        inputDirection =
            Vector3.ClampMagnitude(
                inputDirection,
                1f
            );


        if (inputDirection.magnitude > 0.1f)
        {
            // =================================================
            // CAMERA RELATIVE DIRECTION
            // =================================================

            Vector3 cameraForward =
                cameraTransform.forward;

            Vector3 cameraRight =
                cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection =
                cameraForward *
                inputDirection.z +
                cameraRight *
                inputDirection.x;

            moveDirection.Normalize();


            // =================================================
            // MOVE
            // =================================================

            controller.Move(
                moveDirection *
                crouchSpeed *
                Time.deltaTime
            );


            // =================================================
            // ROTATE
            // =================================================

            transform.forward =
                Vector3.Slerp(
                    transform.forward,
                    moveDirection,
                    rotationSpeed *
                    Time.deltaTime
                );
        }
    }


    // =========================================================
    // CROUCH ANIMATION
    // =========================================================

    void UpdateCrouchAnimation()
    {
        if (!isCrouching)
        {
            animator.SetFloat(
                "CrouchSpeed",
                0f,
                0.1f,
                Time.deltaTime
            );

            return;
        }

        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        float movementAmount =
            new Vector2(
                horizontal,
                vertical
            ).magnitude;

        movementAmount =
            Mathf.Clamp01(
                movementAmount
            );

        animator.SetFloat(
            "CrouchSpeed",
            movementAmount,
            0.1f,
            Time.deltaTime
        );
    }


    // =========================================================
    // SLIDE
    // =========================================================

    void HandleSlide()
    {
        // =====================================================
        // START SLIDE
        // =====================================================

        if (Input.GetKeyDown(slideKey) &&
            isGrounded &&
            !isSliding &&
            !isCrouching &&
            !isHanging &&
            !IsGunOut)
        {
            isSliding = true;

            animator.SetBool(
                "IsSliding",
                true
            );

            SetCharacterControllerSize(
                slideSize
            );
        }


        // =====================================================
        // CONTINUE SLIDE
        // =====================================================

        if (isSliding)
        {
            controller.Move(
                transform.forward *
                slideSpeed *
                Time.deltaTime
            );


            AnimatorStateInfo state =
                animator.GetCurrentAnimatorStateInfo(0);


            if (state.IsName("Slide") &&
                state.normalizedTime >= 0.95f)
            {
                StopSlide();
            }
        }
    }


    // =========================================================
    // STOP SLIDE
    // =========================================================

    private void StopSlide()
    {
        isSliding = false;

        animator.SetBool(
            "IsSliding",
            false
        );

        SetCharacterControllerSize(
            originalSize
        );
    }


    // =========================================================
    // FREEHANG
    // =========================================================

    void HandleFreehang()
    {
        if (!isHanging)
        {
            if (Input.GetKeyDown(hangKey))
            {
                TryStartFreehang();
            }

            return;
        }


        // Stop gravity
        velocity = Vector3.zero;


        // Lock position
        transform.position =
            Vector3.Lerp(
                transform.position,
                hangPosition,
                15f *
                Time.deltaTime
            );


        // Lock rotation
        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                hangRotation,
                15f *
                Time.deltaTime
            );


        // Space = leave hang
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExitFreehang();
        }
    }


    // =========================================================
    // TRY START FREEHANG
    // =========================================================

    void TryStartFreehang()
    {
        if (IsGunOut)
            return;

        if (isCrouching)
            return;

        if (isSliding)
            return;

        if (isHanging)
            return;


        Vector3 rayOrigin =
            transform.position +
            Vector3.up *
            hangHeight;

        Vector3 rayDirection =
            transform.forward;


        if (Physics.Raycast(
            rayOrigin,
            rayDirection,
            out RaycastHit wallHit,
            hangCheckDistance,
            climbableMask))
        {
            Vector3 topRayOrigin =
                wallHit.point +
                Vector3.up *
                2f;


            if (Physics.Raycast(
                topRayOrigin,
                Vector3.down,
                out RaycastHit topHit,
                3f,
                climbableMask))
            {
                float heightDifference =
                    topHit.point.y -
                    transform.position.y;


                if (heightDifference > 0.5f &&
                    heightDifference < 2.5f)
                {
                    StartFreehang(
                        wallHit,
                        topHit
                    );
                }
            }
        }
    }


    // =========================================================
    // START FREEHANG
    // =========================================================

    void StartFreehang(
        RaycastHit wallHit,
        RaycastHit topHit)
    {
        isHanging = true;

        velocity = Vector3.zero;


        Vector3 wallNormal =
            wallHit.normal;

        wallNormal.y = 0f;

        wallNormal.Normalize();


        hangRotation =
            Quaternion.LookRotation(
                -wallNormal
            );


        Vector3 wallPosition =
            wallHit.point +
            wallNormal *
            hangForwardOffset;


        hangPosition =
            new Vector3(
                wallPosition.x,
                topHit.point.y -
                hangHeight,
                wallPosition.z
            );


        controller.enabled = false;

        transform.position =
            hangPosition;

        transform.rotation =
            hangRotation;

        controller.enabled = true;


        animator.SetBool(
            "IsHanging",
            true
        );
    }


    // =========================================================
    // EXIT FREEHANG
    // =========================================================

    void ExitFreehang()
    {
        isHanging = false;

        animator.SetBool(
            "IsHanging",
            false
        );

        velocity.y = 0f;
    }


    // =========================================================
    // GUN SYSTEM
    // =========================================================

    public void SetGunOut(bool value)
    {
        isGunOut = value;


        animator.SetBool(
            "IsGunOut",
            isGunOut
        );


        // =====================================================
        // GUN DRAWN
        // =====================================================

        if (isGunOut)
        {
            // Force crouching player to stand
            if (isCrouching)
            {
                ForceStand();
            }


            // Stop slide
            if (isSliding)
            {
                StopSlide();
            }


            // Stop hanging
            if (isHanging)
            {
                ExitFreehang();
            }


            velocity.y = 0f;
        }
    }


    // =========================================================
    // FORCE STAND
    // =========================================================

    public void ForceStand()
    {
        isCrouching = false;

        animator.SetBool(
            "IsCrouching",
            false
        );

        SetCharacterControllerSize(
            originalSize
        );
    }


    // =========================================================
    // GRAVITY
    // =========================================================

    void ApplyGravity()
    {
        if (isHanging)
            return;


        velocity.y +=
            Physics.gravity.y *
            Time.deltaTime;


        controller.Move(
            velocity *
            Time.deltaTime
        );
    }


    // =========================================================
    // CHARACTER CONTROLLER SIZE
    // =========================================================

    private void SetCharacterControllerSize(
        CharControllerSize size)
    {
        controller.center =
            size.centre;

        controller.height =
            size.height;
    }


    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundDistance
            );
        }


        Gizmos.DrawRay(
            transform.position +
            Vector3.up *
            hangHeight,

            transform.forward *
            hangCheckDistance
        );
    }
}


// =============================================================
// CHARACTER CONTROLLER SIZE
// =============================================================

[System.Serializable]
public struct CharControllerSize
{
    public Vector3 centre;
    public float height;
}