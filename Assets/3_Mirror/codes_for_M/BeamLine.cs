using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamLine : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        lr.positionCount = 2;
    }

    void Update()
    {
        lr.SetPosition(0, startPoint.position);
        lr.SetPosition(1, endPoint.position);
    }
}