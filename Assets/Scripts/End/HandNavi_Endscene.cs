using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandNavi_Endscene : MonoBehaviour
{
    // Start is called before the first frame update
    private XRGrabInteractable grabInteractable;
    public GameObject Hand_Navi;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);
    
    }
    public void SetupPose(BaseInteractionEventArgs arg)
    {
        
        if (arg.interactorObject is XRDirectInteractor)
        {
            Hand_Navi.SetActive(false);

        }
    }
    public void UnSetPose(BaseInteractionEventArgs arg)
    {

        if (arg.interactorObject is XRDirectInteractor)
        {

            //_rigidbody.isKinematic = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
