using System.Collections;
using UnityEngine;

public class LineAnimate : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float shootSpeed = 3f;
    private Vector3[] points;
    public void Init()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Store all LineRenderer positions
        points = new Vector3[lineRenderer.positionCount];

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            points[i] = lineRenderer.GetPosition(i);
            lineRenderer.SetPosition(i, points[0]); // Hide line at start
        }
    }
    public IEnumerator AnimateLine()
    {
        // Reset all points to start point
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, points[0]);
        }

        // Animate point by point
        for (int segment = 1; segment < points.Length; segment++)
        {
            Vector3 startPos = points[segment - 1];
            Vector3 endPos = points[segment];

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * shootSpeed;

                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

                // Keep previous points fixed
                for (int i = 0; i < segment; i++)
                {
                    lineRenderer.SetPosition(i, points[i]);
                }

                // Move current point
                lineRenderer.SetPosition(segment, currentPos);

                yield return null;
            }

            lineRenderer.SetPosition(segment, endPos);
        }
    }
}
