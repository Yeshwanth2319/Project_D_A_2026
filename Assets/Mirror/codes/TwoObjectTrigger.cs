using UnityEngine;

public class LineTrigger_1 : MonoBehaviour
{
    [Header("Line Renderer Objects")]
    public string lineObject1Name;
    public string lineObject2Name;

    [Header("Object to Appear")]
    public GameObject objectToAppear;

    private bool line1Touched;
    private bool line2Touched;

    private void Start()
    {
        objectToAppear.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == lineObject1Name)
        {
            line1Touched = true;
        }

        if (other.gameObject.name == lineObject2Name)
        {
            line2Touched = true;
        }

        // Both objects touched the Box Collider
        if (line1Touched && line2Touched)
        {
            objectToAppear.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == lineObject1Name)
        {
            line1Touched = false;
        }

        if (other.gameObject.name == lineObject2Name)
        {
            line2Touched = false;
        }
    }
}