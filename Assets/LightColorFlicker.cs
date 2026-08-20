using UnityEngine;

public class LightColorFlicker : MonoBehaviour
{
    public Light pointLight;

    [Header("Intensity")]
    public float minIntensity = 1f;
    public float maxIntensity = 3f;

    [Header("Change Speed")]
    public float changeInterval = 0.2f;

    // Colors the light will cycle through
    public Color[] colors =
    {
        Color.cyan,
        Color.blue,
        Color.red,
        Color.green
    };

    void Start()
    {
        if (pointLight == null)
            pointLight = GetComponent<Light>();

        InvokeRepeating(nameof(ChangeLight), 0f, changeInterval);
    }

    void ChangeLight()
    {
        // Random brightness
        pointLight.intensity = Random.Range(minIntensity, maxIntensity);

        // Random color
        pointLight.color = colors[Random.Range(0, colors.Length)];
    }
}