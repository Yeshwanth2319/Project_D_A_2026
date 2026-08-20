using UnityEngine;

public class MirrorTrigger : MonoBehaviour
{
    public MirrorPush mirror;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mirror.playerInside = true;
            mirror.player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mirror.playerInside = false;
            mirror.player = null;
        }
    }
}