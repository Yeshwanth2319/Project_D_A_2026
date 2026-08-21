using UnityEngine;

public class MechanismHandle : MonoBehaviour
{
    public GodOfWarMechanism mechanism;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        mechanism.SetPlayerInside(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        mechanism.SetPlayerInside(false);
    }
}