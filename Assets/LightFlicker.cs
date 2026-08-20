using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light pointLight;

    [Header("Light Settings")]
    public float minIntensity = 1f;
    public float maxIntensity = 100f;

    [Header("Flicker Speed")]
    public float flickerSpeed = 0.05f;

    void Start()
    {
        if (pointLight == null)
            pointLight = GetComponent<Light>();

        InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
    }

    void Flicker()
    {
        pointLight.intensity = Random.Range(minIntensity, maxIntensity);
    }
}