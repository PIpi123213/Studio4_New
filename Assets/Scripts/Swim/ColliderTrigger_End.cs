using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger_End : MonoBehaviour
{
    // Start is called before the first frame update
    public EndFishAnimation fishAnimation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fishAnimation.StartAnimation();

        }



    }
}
