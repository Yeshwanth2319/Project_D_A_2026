using UnityEngine;
using UnityEngine.Splines;

public class CenterLightController : MonoBehaviour
{
    public Light crystalLight;

    public LineAnimate leftBeam;
    public LineAnimate rightBeam;

    private bool playerNear = false;

    private bool activated = false;

    public GameObject Glow;

    public GameObject Triggers;
    private void Start()
    {
        leftBeam.Init();
        rightBeam.Init();
        leftBeam.gameObject.SetActive(false);
        rightBeam.gameObject.SetActive(false);
    }
    void Update()
    {
        if (playerNear &&
           !activated &&
           Input.GetKeyDown(KeyCode.E))
        {

            ActivateLight();
            Glow.SetActive(true);
        }
    }

    void ActivateLight()
    {
        activated = true;

        crystalLight.enabled = true;

        leftBeam.gameObject.SetActive(true);
        rightBeam.gameObject.SetActive(true);
        StartCoroutine(leftBeam.AnimateLine());
        StartCoroutine(rightBeam.AnimateLine());

        Triggers.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}