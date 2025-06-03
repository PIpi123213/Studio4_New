using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public ShaderController_Stage0 trigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ÓÎÏ·¿ªÊ¼");
            trigger.Startgame();


        }



    }
}
