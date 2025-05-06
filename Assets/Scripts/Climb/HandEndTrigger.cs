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
    void End()
    {
        var interactors = new List<IXRSelectInteractor>(ClimbInteractable1.interactorsSelecting);
        foreach (var interactor in interactors)
        {
            interactionManager.SelectExit(interactor, ClimbInteractable1);
        }
        StaticPoint.target = StaticPoint.gameObject.transform;
        
        StaticPoint.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
        ClimbInteractable1.interactionLayers = 0;

    }


}
