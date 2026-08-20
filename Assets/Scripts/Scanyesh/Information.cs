using UnityEngine;

public class Information : MonoBehaviour
{
    [Header("Gun")]
    public GameObject gunObject;

    [Header("Animator")]
    public Animator animator;

    [Header("Animation Parameters")]
    public string drawTrigger = "DrawGun";
    public string putBackTrigger = "PutGunBack";

    [Header("Animation States")]
    public string drawStateName = "GunDraw";
    public string putBackStateName = "GunPutBack";

    [Header("Scan")]
    public float scanDistance = 15f;
    public LayerMask scanLayer;

    [Header("Input")]
    public KeyCode gunKey = KeyCode.Q;

    private Camera cam;

    private bool gunEquipped = false;
    private bool gunAnimating = false;

    private void Start()
    {
        cam = Camera.main;

        if (gunObject != null)
        {
            gunObject.SetActive(false);
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // Q = Equip / Unequip
        if (Input.GetKeyDown(gunKey))
        {
            ToggleGun();
        }

        // Check draw / put-back animation
        CheckAnimationFinished();

        // Only scan when gun is fully equipped
        if (gunEquipped && !gunAnimating)
        {
            ScanObject();
        }
    }

    // =========================================================
    // GUN TOGGLE
    // =========================================================

    void ToggleGun()
    {
        // Don't allow Q while draw animation is playing
        if (gunAnimating)
            return;

        if (!gunEquipped)
        {
            EquipGun();
        }
        else
        {
            PutGunBack();
        }
    }

    // =========================================================
    // EQUIP GUN
    // =========================================================

    void EquipGun()
    {
        gunEquipped = true;
        gunAnimating = true;

        // Show gun
        if (gunObject != null)
        {
            gunObject.SetActive(true);
        }

        // Make sure reverse trigger is cleared
        animator.ResetTrigger(putBackTrigger);

        // Play GunDraw FORWARD
        animator.SetTrigger(drawTrigger);
    }

    // =========================================================
    // PUT GUN BACK
    // =========================================================

    void PutGunBack()
    {
        gunEquipped = false;
        gunAnimating = true;

        // Hide scanner UI immediately
        if (ScannerUI.Instance != null)
        {
            ScannerUI.Instance.HideInfo();
        }

        // Make sure draw trigger is cleared
        animator.ResetTrigger(drawTrigger);

        // Play same GunDraw animation BACKWARD
        animator.SetTrigger(putBackTrigger);
    }

    // =========================================================
    // CHECK ANIMATION
    // =========================================================

    void CheckAnimationFinished()
    {
        if (animator == null)
            return;

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        // -----------------------------------------------------
        // GUN DRAW
        // -----------------------------------------------------

        if (state.IsName(drawStateName))
        {
            // Animation reached the end
            if (state.normalizedTime >= 0.95f)
            {
                gunAnimating = false;
            }
        }

        // -----------------------------------------------------
        // GUN PUT BACK
        // -----------------------------------------------------

        if (state.IsName(putBackStateName))
        {
            // Because animation speed is -1,
            // normalizedTime goes DOWN toward 0.
            if (state.normalizedTime <= 0.05f)
            {
                gunAnimating = false;

                // Hide gun AFTER reverse animation finishes
                if (gunObject != null)
                {
                    gunObject.SetActive(false);
                }
            }
        }
    }

    // =========================================================
    // SCANNER
    // =========================================================

    void ScanObject()
    {
        if (cam == null)
            return;

        Ray ray = cam.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            scanDistance,
            scanLayer))
        {
            ScannableObject obj =
                hit.collider.GetComponent<ScannableObject>();

            if (obj != null)
            {
                if (ScannerUI.Instance != null)
                {
                    ScannerUI.Instance.ShowInfo(
                        obj.objectName,
                        obj.infoText
                    );
                }
            }
            else
            {
                HideScannerUI();
            }
        }
        else
        {
            HideScannerUI();
        }
    }

    // =========================================================
    // HIDE UI
    // =========================================================

    void HideScannerUI()
    {
        if (ScannerUI.Instance != null)
        {
            ScannerUI.Instance.HideInfo();
        }
    }
}