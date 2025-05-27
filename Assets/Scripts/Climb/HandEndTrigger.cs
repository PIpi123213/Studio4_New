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
    public GameObject model;
    public Collider model_coll;
    public Rigidbody model_rigibody;
    public GameObject scene;
    public GameObject scenetool;
    public GameObject rock;
    void Start()
    {
        //ClimbInteractable1 = GetComponent<CustomClimbInteractable>();
        model.SetActive(false);

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
        model.SetActive(true);
        scenetool.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        handend.SetActive(false);
       // model_coll.isTrigger = false;
        model_rigibody.isKinematic = false;

        yield return new WaitForSeconds(1.5f);
        trigger2.Endanimation();
        yield return new WaitForSeconds(3f);
        rock.SetActive(false);
        StaticPoint.target = StaticPoint.gameObject.transform;
        StaticPoint.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
        yield return null;

        yield return new WaitForSeconds(2f);
        scene.SetActive(false);
        
    }


}
