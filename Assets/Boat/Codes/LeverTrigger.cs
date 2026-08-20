using UnityEngine;

public class LineTrigger : MonoBehaviour
{
    [Header("Line Objects")]
    public GameObject lineObject1;
    public GameObject lineObject2;

    [Header("Third Line")]
    public GameObject thirdLine;

    private bool line1Touched;
    private bool line2Touched;

    private void Start()
    {
        thirdLine.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == lineObject1)
        {
            line1Touched = true;
        }

        if (other.gameObject == lineObject2)
        {
            line2Touched = true;
        }

        // Both lines are complete
        if (line1Touched && line2Touched)
        {
            thirdLine.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == lineObject1)
        {
            line1Touched = false;
        }

        if (other.gameObject == lineObject2)
        {
            line2Touched = false;
        }
    }
}