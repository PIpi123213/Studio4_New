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
    public GameObject handend;
    public Trigger2 trigger2 ;
    void Start()
    {
        //ClimbInteractable1 = GetComponent<CustomClimbInteractable>();

     
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
        StartCoroutine(End());
    }
    private IEnumerator End()
    {

        yield return new WaitForSeconds(0.3f);
        handend.SetActive(false);


        //ClimbInteractable1.interactionLayers = 0;
        trigger2.Endanimation();

        StaticPoint.target = StaticPoint.gameObject.transform;
        StaticPoint.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
        yield return null;

    }


}
