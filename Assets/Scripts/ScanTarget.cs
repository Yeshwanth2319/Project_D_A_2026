using UnityEngine;

public class ScanTarget : MonoBehaviour
{
    [Header("Information")]
    public string objectName = "Unknown Object";

    [TextArea(3, 10)]
    public string information =
        "No information available.";
}