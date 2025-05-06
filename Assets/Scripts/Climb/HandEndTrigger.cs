using Obi;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandEndTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public ObiParticleAttachment StaticPoint;
    private XRInteractionManager interactionManager;
    private InteractionLayerMask originalLayer;
    public CustomClimbInteractable ClimbInteractable1;
    public Trigger2 trigger2 ;
    void Start()
    {
        ClimbInteractable1 = GetComponent<CustomClimbInteractable>();

     
        ClimbInteractable1.selectEntered.AddListener(OnSelectEnter);
        interactionManager = ClimbInteractable1.interactionManager;

        // 记录原始交互层
        originalLayer = ClimbInteractable1.interactionLayers;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        End();
    }
    private void End()
    {
        ClimbInteractable1.enabled = false;



        
        //ClimbInteractable1.interactionLayers = 0;
        trigger2.Endanimation();

        StaticPoint.target = StaticPoint.gameObject.transform;
        StaticPoint.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;


    }


}
