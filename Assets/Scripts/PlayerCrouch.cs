using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Animator animator;

    [Header("Crouch Settings")]
    public KeyCode crouchKey = KeyCode.C;
    public float crouchSpeed = 2f;
    public float rotationSpeed = 10f;
    public CharControllerSize crouchSize;
    private CharControllerSize originalSize;
    private bool isCrouching;

    public bool IsCrouching
    {
        get { return isCrouching; }
    }
    void Start()
    {
        
        originalSize.centre = controller.center;
        originalSize.height = controller.height;
    }

    void Update()
    {
        HandleCrouchToggle();
        HandleCrouchMovement();
        UpdateCrouchAnimation();
    }

    void HandleCrouchToggle()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;

            animator.SetBool("IsCrouching", isCrouching);
        }
    }

    void HandleCrouchMovement()
    {
        if (!isCrouching)
            return;
        
        SetCharacterControllerSize(crouchSize);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(
            horizontal,
            0f,
            vertical
        ).normalized;

        if (direction.magnitude > 0.1f)
        {
            controller.Move(
                direction * crouchSpeed * Time.deltaTime
            );

            transform.forward = Vector3.Slerp(
                transform.forward,
                direction,
                rotationSpeed * Time.deltaTime);

            SetCharacterControllerSize(originalSize);
            
        }
    }

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

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float movementAmount = new Vector2(
            horizontal,
            vertical
        ).magnitude;

        movementAmount = Mathf.Clamp01(movementAmount);

        animator.SetFloat(
            "CrouchSpeed",
            movementAmount,
            0.1f,
            Time.deltaTime
        );
    }
    private void SetCharacterControllerSize(CharControllerSize charControllerSize)
    {
        controller.center = charControllerSize.centre;
        controller.height = charControllerSize.height;
    }
    [System.Serializable]
    public struct CharControllerSize
    {
        public Vector3 centre;
        public float height;
    }
}