using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform TrackingObject;
    void Start()
    {


        

    }

    public void fixRotation()
    {

        StartCoroutine(fixrotation());

    }
    private IEnumerator fixrotation()
    {
       
        yield return new WaitForSeconds(0.1f);
        float targetY = -TrackingObject.localRotation.eulerAngles.y;
        transform.localRotation = Quaternion.Euler(0f, targetY, 0f);
    }
    // Update is called once per frame

}
