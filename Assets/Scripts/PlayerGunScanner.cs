using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerGunScanner : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    public Animator animator;
    public PlayerMovement playerMovement;
    public Transform cameraTransform;


    // =========================================================
    // GUN
    // =========================================================

    [Header("Gun")]
    public GameObject gunObject;


    // =========================================================
    // HOLOGRAM UI
    // =========================================================

    [Header("Hologram UI")]
    public GameObject hologramPanel;
    public TMP_Text objectNameText;
    public TMP_Text informationText;


    // =========================================================
    // SCANNING
    // =========================================================

    [Header("Scan Settings")]
    public float scanDistance = 15f;
    public LayerMask scanLayer;


    // =========================================================
    // INPUT
    // =========================================================

    [Header("Input")]
    public KeyCode gunKey = KeyCode.Q;


    // =========================================================
    // GUN ANIMATOR LAYER
    // =========================================================

    [Header("Gun Layer")]
    public int gunLayerIndex = 1;


    // =========================================================
    // STATE
    // =========================================================

    private bool gunOut = false;
    private bool gunChanging = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();


        // Gun starts hidden.
        if (gunObject != null)
            gunObject.SetActive(false);


        // Hologram starts hidden.
        if (hologramPanel != null)
            hologramPanel.SetActive(false);


        // IMPORTANT:
        // Gun layer starts completely OFF.
        animator.SetLayerWeight(
            gunLayerIndex,
            0f
        );
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        HandleGunInput();

        if (gunOut)
        {
            HandleScanning();
        }
        else
        {
            HideHologram();
        }
    }


    // =========================================================
    // Q INPUT
    // =========================================================

    private void HandleGunInput()
    {
        if (!Input.GetKeyDown(gunKey))
            return;


        // Prevent Q from being pressed again
        // while draw/holster animation is playing.
        if (gunChanging)
            return;


        if (!gunOut)
        {
            DrawGun();
        }
        else
        {
            StartCoroutine(
                HolsterGun()
            );
        }
    }


    // =========================================================
    // DRAW GUN
    // =========================================================

    private void DrawGun()
    {
        gunChanging = true;

        gunOut = true;


        // Show gun.
        if (gunObject != null)
            gunObject.SetActive(true);


        // Tell movement script that gun is active.
        if (playerMovement != null)
        {
            playerMovement.SetGunOut(true);
        }


        // =====================================================
        // TURN GUN LAYER ON
        // =====================================================

        animator.SetLayerWeight(
            gunLayerIndex,
            1f
        );


        // =====================================================
        // PLAY DRAW ANIMATION
        // =====================================================

        animator.CrossFadeInFixedTime(
            "GunDraw",
            0.05f,
            gunLayerIndex,
            0f
        );


        StartCoroutine(
            FinishDraw()
        );
    }


    // =========================================================
    // FINISH DRAW
    // =========================================================

    private IEnumerator FinishDraw()
    {
        // Give Animator one frame to enter GunDraw.
        yield return null;


        // Wait until GunDraw finishes.
        while (true)
        {
            AnimatorStateInfo state =
                animator.GetCurrentAnimatorStateInfo(
                    gunLayerIndex
                );


            if (state.IsName("GunDraw") &&
                state.normalizedTime >= 0.95f)
            {
                break;
            }


            yield return null;
        }


        // Gun is now ready.
        gunChanging = false;
    }


    // =========================================================
    // HOLSTER GUN
    // =========================================================

    private IEnumerator HolsterGun()
    {
        gunChanging = true;


        HideHologram();


        // =====================================================
        // PLAY GUN BACK ANIMATION
        // =====================================================

        animator.CrossFadeInFixedTime(
            "GunDraw back",
            0.05f,
            gunLayerIndex,
            0f
        );


        // =====================================================
        // WAIT FOR BACK ANIMATION
        // =====================================================

        yield return null;


        float timer = 0f;

        while (timer < 3f)
        {
            AnimatorStateInfo state =
                animator.GetCurrentAnimatorStateInfo(
                    gunLayerIndex
                );


            // Make sure we are actually in the
            // GunDraw back state.
            if (state.IsName("GunDraw back"))
            {
                if (state.normalizedTime >= 0.95f)
                {
                    break;
                }
            }


            timer += Time.deltaTime;

            yield return null;
        }


        // =====================================================
        // THIS IS THE IMPORTANT PART
        // =====================================================

        // Turn Gun Layer completely OFF.
        animator.SetLayerWeight(
            gunLayerIndex,
            0f
        );


        // =====================================================
        // RESET PLAYER STATE
        // =====================================================

        gunOut = false;


        if (playerMovement != null)
        {
            playerMovement.SetGunOut(false);
        }


        // Hide gun.
        if (gunObject != null)
        {
            gunObject.SetActive(false);
        }


        gunChanging = false;
    }


    // =========================================================
    // SCANNING
    // =========================================================

    private void HandleScanning()
    {
        if (cameraTransform == null)
            return;


        Ray ray = new Ray(
            cameraTransform.position,
            cameraTransform.forward
        );


        RaycastHit hit;


        if (Physics.Raycast(
            ray,
            out hit,
            scanDistance,
            scanLayer))
        {
            ScanTarget target =
                hit.collider.GetComponent<ScanTarget>();


            if (target != null)
            {
                ShowInformation(target);
                return;
            }
        }


        HideHologram();
    }


    // =========================================================
    // SHOW HOLOGRAM
    // =========================================================

    private void ShowInformation(
        ScanTarget target)
    {
        if (hologramPanel == null)
            return;


        hologramPanel.SetActive(true);


        if (objectNameText != null)
        {
            objectNameText.text =
                target.objectName;
        }


        if (informationText != null)
        {
            informationText.text =
                target.information;
        }
    }


    // =========================================================
    // HIDE HOLOGRAM
    // =========================================================

    private void HideHologram()
    {
        if (hologramPanel != null)
        {
            hologramPanel.SetActive(false);
        }
    }


    // =========================================================
    // PUBLIC GUN STATE
    // =========================================================

    public bool IsGunOut
    {
        get
        {
            return gunOut;
        }
    }
}