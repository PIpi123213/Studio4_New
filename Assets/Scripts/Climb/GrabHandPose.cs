using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class GrabHandPose : MonoBehaviour
{
    public bool ifTwoHandGrab = false;
    public int HandGrabing = 0;
    public handDataPose rightHandPose;
    public handDataPose leftHandPose;
    public GameObject leftHandModel_Geom;
    public GameObject rightHandModel_Geom;
    private Vector3 startingHandPosition_left;
    private Vector3 finalHandPosition_left;
    private Quaternion startingHandRotation_left;
    private Quaternion finalHandRotation_left;
    private Quaternion[] startingFingerRotations_left;
    private Quaternion[] finalFingerRotations_left;

    public float poseTransitionDuration = 0.2f;

    private Vector3 startingHandPosition_right;
    private Vector3 finalHandPosition_right;
    private Quaternion finalHandRotation_right;
    private Quaternion startingHandRotation_right;
    private Quaternion[] startingFingerRotations_right;
    private Quaternion[] finalFingerRotations_right;


    private XRGrabInteractable grabInteractable;
    void Start()
    {
         grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);

        rightHandPose.gameObject.SetActive(false);
        leftHandPose.gameObject.SetActive(false);
        //StartCoroutine(findGeom());
    }

    public void SetupPose(BaseInteractionEventArgs arg)
    {
        if (grabInteractable.interactorsSelecting.Count == 2)
        {
            HandGrabing = 2;
        }
        else
        {
            HandGrabing = 1;
        }
        if (arg.interactorObject is XRDirectInteractor)
        {
            handDataPose handData = arg.interactorObject.transform.GetComponentInChildren<handDataPose>();
            handData.animator.enabled = false;
            if(handData.type == handDataPose.HandModelType.Right && rightHandPose!=null)
            {
                SetRightHandDataValues(handData, rightHandPose);
               // SendHandData(handData, finalHandPosition_right, finalHandRotation_right, finalFingerRotations_right);
               

                StartCoroutine(SetHandDataRoutine(handData, finalHandPosition_right, finalHandRotation_right, finalFingerRotations_right, startingHandPosition_right, startingHandRotation_right, startingFingerRotations_right));

                
                    rightHandPose.gameObject.SetActive(true);
                    rightHandModel_Geom.SetActive(false);
                
                
                
            }
            else if(handData.type == handDataPose.HandModelType.Left && leftHandPose != null)
            {
                SetLeftHandDataValues(handData, leftHandPose);
                // SendHandData(handData, finalHandPosition_left, finalHandRotation_left, finalFingerRotations_left);
                StartCoroutine(SetHandDataRoutine(handData, finalHandPosition_left, finalHandRotation_left, finalFingerRotations_left, startingHandPosition_left, startingHandRotation_left, startingFingerRotations_left));
                
                    leftHandPose.gameObject.SetActive(true);
                    leftHandModel_Geom.SetActive(false);
                
               
                //Debug.LogWarning("leftshould"+ leftHandPose.transform.position + "final" + finalHandPosition_left + "left" + handData.root.position);
            }

            
          
        }
    }

    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        if (grabInteractable.interactorsSelecting.Count == 1)
        {
            HandGrabing = 1;
        }
        else
        {
            HandGrabing = 0;
        }
        if (arg.interactorObject is XRDirectInteractor)
        {
            handDataPose handData = arg.interactorObject.transform.GetComponentInChildren<handDataPose>();
            if (handData.type == handDataPose.HandModelType.Right && rightHandPose != null)
            {
                //SendHandData(handData, startingHandPosition_right, startingHandRotation_right, startingFingerRotations_right);
                StartCoroutine(SetHandDataRoutine(handData, startingHandPosition_right, startingHandRotation_right, startingFingerRotations_right, finalHandPosition_right, finalHandRotation_right, finalFingerRotations_right));
                SetHandDataBack_Right();
                    rightHandPose.gameObject.SetActive(false);
                    rightHandModel_Geom.SetActive(true);
                

            }
            else if (handData.type == handDataPose.HandModelType.Left && leftHandPose != null)
            {
                //SendHandData(handData, startingHandPosition_left, startingHandRotation_left, startingFingerRotations_left);
                StartCoroutine(SetHandDataRoutine(handData, startingHandPosition_left, startingHandRotation_left, startingFingerRotations_left,finalHandPosition_left, finalHandRotation_left, finalFingerRotations_left));
                SetHandDataBack_Left();
                    leftHandPose.gameObject.SetActive(false);
                    leftHandModel_Geom.SetActive(true);
                

            }
            handData.animator.enabled = true;

          
          
        }
    }
    public void SetLeftHandDataValues(handDataPose h1, handDataPose h2)
    {
        startingHandPosition_left = new Vector3(h1.root.localPosition.x / h1.root.localScale.x,
            h1.root.localPosition.y / h1.root.localScale.y, h1.root.localPosition.z / h1.root.localScale.z);
        finalHandPosition_left = new Vector3(h2.root.localPosition.x / h2.root.localScale.x,
            h2.root.localPosition.y / h2.root.localScale.y, h2.root.localPosition.z / h2.root.localScale.z);

        //startingHandPosition_left = h1.orginLocalPos;

        startingHandRotation_left = h1.root.localRotation;
        finalHandRotation_left = h2.root.localRotation;

        startingFingerRotations_left = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations_left = new Quaternion[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotations_left[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations_left[i] = h2.fingerBones[i].localRotation;
        }
    }
    public void SetRightHandDataValues(handDataPose h1, handDataPose h2)
    {
        startingHandPosition_right = new Vector3(h1.root.localPosition.x / h1.root.localScale.x,
            h1.root.localPosition.y / h1.root.localScale.y, h1.root.localPosition.z / h1.root.localScale.z);
        finalHandPosition_right = new Vector3(h2.root.localPosition.x / h2.root.localScale.x,
            h2.root.localPosition.y / h2.root.localScale.y, h2.root.localPosition.z / h2.root.localScale.z);

        //startingHandPosition_right = h1.orginLocalPos;


        startingHandRotation_right = h1.root.localRotation;
        finalHandRotation_right = h2.root.localRotation;

        startingFingerRotations_right = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations_right = new Quaternion[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotations_right[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations_right[i] = h2.fingerBones[i].localRotation;
        }
    }
    public void SetHandDataBack_Right()
    {
        startingHandPosition_right = Vector3.zero;

        finalHandPosition_right = Vector3.zero;
           

        //startingHandPosition_right = h1.orginLocalPos;


        startingHandRotation_right = Quaternion.identity;
        finalHandRotation_right = Quaternion.identity;

        startingFingerRotations_right = null;
        finalFingerRotations_right = null;

     
    }
    public void SetHandDataBack_Left()
    {
        startingHandPosition_left = Vector3.zero;

        finalHandPosition_left = Vector3.zero;


        //startingHandPosition_right = h1.orginLocalPos;


        startingHandRotation_left = Quaternion.identity;
        finalHandRotation_left = Quaternion.identity;

        startingFingerRotations_left = null;
        finalFingerRotations_left = null;


    }

    public void SendHandData(handDataPose h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
    }
    public IEnumerator SetHandDataRoutine(handDataPose h,Vector3 newPosition,Quaternion newRotation, Quaternion[] newBonesRotation, Vector3 startingPosition, Quaternion startingRotation, Quaternion[] startingBonesRotation)
    {
        float timer = 0;
        while (timer < poseTransitionDuration)
        {
            Vector3 p = Vector3.Lerp(startingPosition, newPosition, timer / poseTransitionDuration);
            Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);
            h.root.localPosition = p;
            h.root.localRotation = r;
            for(int i = 0; i< newBonesRotation.Length; i++)
            {
                h.fingerBones[i].localRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i],timer/poseTransitionDuration);
            }



            timer += Time.deltaTime;
            yield return null;

        }


    }
    private IEnumerator findGeom()
    {
        yield return null;
        // 等待一帧
        leftHandModel_Geom = GameObject.FindWithTag("LeftHandGem");
        if (leftHandModel_Geom == null)
            Debug.LogError("仍然未找到！");

        rightHandModel_Geom = GameObject.FindWithTag("RightHandGem");
        if (rightHandModel_Geom == null)
            Debug.LogError("仍然未找到！");
    }



    // Update is called once per frame

}
