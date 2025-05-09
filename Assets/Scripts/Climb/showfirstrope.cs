using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showfirstrope : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject rope;
    public GameObject trigger;
    public Collider trigger_coll;

    void Start()
    {
        rope.SetActive(false);
        trigger_coll.isTrigger = true;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ropeshow()
    {
        rope.SetActive(true);
        trigger_coll.isTrigger = false;
    }
}
