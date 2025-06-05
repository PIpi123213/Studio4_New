using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrantoEndPosition : MonoBehaviour
{
    // Start is called before the first frame update

    public Rigidbody playerTrans;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransToEnd()
    {
        playerTrans.position = transform.position;

        playerTrans.velocity = Vector3.zero;


    }
}
