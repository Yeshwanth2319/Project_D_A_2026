using System.Collections;
using UnityEngine;

public class ActivateTarget : MonoBehaviour
{
    public string movableObjectName;
    public LineAnimate lineAnimate;

    private void Start()
    {
        lineAnimate.Init();
        lineAnimate.gameObject.SetActive(false);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == movableObjectName)
        {
            lineAnimate.gameObject.SetActive(true);
            StartCoroutine(lineAnimate.AnimateLine());
        }
    }
    
}