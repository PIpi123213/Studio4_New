using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static Oculus.Interaction.GrabAPI.FingerPalmGrabAPI;

public class HandPoseSlider : MonoBehaviour
{
    // Start is called before the first frame update
    public bool ifTwoHandGrab = false;
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

    private handDataPose pendingRightHandData = null;
    private handDataPose pendingLeftHandData = null;

    // ��ʶ��ǰ�Ƿ��� zipline ״̬�������� Zipline �ű����ƣ�
    public bool ziplineActive = false;
    public bool isUnset = false;

    void Start()
    {
        //XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        CustomClimbInteractable grabInteractable = GetComponent<CustomClimbInteractable>();


        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);

        rightHandPose.gameObject.SetActive(false);
        leftHandPose.gameObject.SetActive(false);
    }

    public void SetupPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRDirectInteractor)
        {
            handDataPose handData = arg.interactorObject.transform.GetComponentInChildren<handDataPose>();
            //handData.animator.enabled = false;
            if (handData.type == handDataPose.HandModelType.Right && rightHandPose != null)
            {
                //SetRightHandDataValues(handData, rightHandPose);
                //SendHandData(handData, finalHandPosition_right, finalHandRotation_right, finalFingerRotations_right);


                //StartCoroutine(SetHandDataRoutine(handData, finalHandPosition_right, finalHandRotation_right, finalFingerRotations_right, startingHandPosition_right, startingHandRotation_right, startingFingerRotations_right));

               
                    rightHandPose.gameObject.SetActive(true);
                    rightHandModel_Geom.SetActive(false);
                


            }
            else if (handData.type == handDataPose.HandModelType.Left && leftHandPose != null)
            {
                //SetLeftHandDataValues(handData, leftHandPose);
               // SendHandData(handData, finalHandPosition_left, finalHandRotation_left, finalFingerRotations_left);
               // StartCoroutine(SetHandDataRoutine(handData, finalHandPosition_left, finalHandRotation_left, finalFingerRotations_left, startingHandPosition_left, startingHandRotation_left, startingFingerRotations_left));
               
                    leftHandPose.gameObject.SetActive(true);
                    leftHandModel_Geom.SetActive(false);
                

                //Debug.LogWarning("leftshould"+ leftHandPose.transform.position + "final" + finalHandPosition_left + "left" + handData.root.position);
            }



        }
    }

    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        Debug.LogWarning("�����˳��¼�");
        // ������� zipline ״̬����ֱ�Ӵ����˳��������ݴ��ֵ�����
        if (ziplineActive)
        {
            handDataPose handData = arg.interactorObject.transform.GetComponentInChildren<handDataPose>();
            if (handData != null)
            {
                if (handData.type == handDataPose.HandModelType.Right)
                {
                    pendingRightHandData = handData;
                    Debug.Log("�ݴ������˳��¼���Ϣ");
                }
                else if (handData.type == handDataPose.HandModelType.Left)
                {
                    pendingLeftHandData = handData;
                    Debug.Log("�ݴ������˳��¼���Ϣ");
                }
                isUnset = true;
            }
            return;
        }
        else
        {
            if (arg.interactorObject is XRDirectInteractor)
            {

                Debug.LogWarning("songkai");
                handDataPose handData = arg.interactorObject.transform.GetComponentInChildren<handDataPose>();
                if (handData.type == handDataPose.HandModelType.Right && rightHandPose != null)
                {
                    ResetPose(handData);
                    //SendHandData(handData, startingHandPosition_right, startingHandRotation_right, startingFingerRotations_right);
                    //StartCoroutine(SetHandDataRoutine(handData, startingHandPosition_right, startingHandRotation_right, startingFingerRotations_right, finalHandPosition_right, finalHandRotation_right, finalFingerRotations_right));

                    rightHandPose.gameObject.SetActive(false);
                        rightHandModel_Geom.SetActive(true);
                    

                }
                else if (handData.type == handDataPose.HandModelType.Left && leftHandPose != null)
                {
                    //SendHandData(handData, startingHandPosition_left, startingHandRotation_left, startingFingerRotations_left);
                    //StartCoroutine(SetHandDataRoutine(handData, startingHandPosition_left, startingHandRotation_left, startingFingerRotations_left, finalHandPosition_left, finalHandRotation_left, finalFingerRotations_left));

                    ResetPose(handData);
                    leftHandPose.gameObject.SetActive(false);
                        leftHandModel_Geom.SetActive(true);
                    

                }
                handData.animator.enabled = true;
            }
        }
        // �� zipline ״̬�£�ֱ�Ӵ����˳���
    }
    public void ResetPose(handDataPose handData)
    {
        handData.root.localPosition = handData.orginLocalPos;
        Debug.Log(handData.root.localPosition);


    }
    public void ProcessPendingExitEvents()
    {
        // ����ݴ��������˳���Ϣ
        if (pendingRightHandData != null)
        {
            ResetPose(pendingRightHandData);
            ProcessUnSetPoseForHand(pendingRightHandData);
           
            pendingRightHandData = null;
        }
        // ͬ��������
        if (pendingLeftHandData != null)
        {
            ResetPose(pendingLeftHandData);
            ProcessUnSetPoseForHand(pendingLeftHandData);
            pendingLeftHandData = null;
        }
    }
    private void ProcessUnSetPoseForHand(handDataPose handData)
    {
        Debug.LogWarning("�����ݴ��˳��¼����֣�" + handData.type);
        
        if (handData.type == handDataPose.HandModelType.Right && rightHandPose != null)
        {
            // �˴��Ĳ������Ը���ʵ���������
            //StartCoroutine(SetHandDataRoutine(handData, startingHandPosition_right, startingHandRotation_right, startingFingerRotations_right, finalHandPosition_right, finalHandRotation_right, finalFingerRotations_right));
            
                rightHandPose.gameObject.SetActive(false);
                rightHandModel_Geom.SetActive(true);
            
        }
        else if (handData.type == handDataPose.HandModelType.Left && leftHandPose != null)
        {
            //StartCoroutine(SetHandDataRoutine(handData, startingHandPosition_left, startingHandRotation_left, startingFingerRotations_left, finalHandPosition_left, finalHandRotation_left, finalFingerRotations_left));
            
                leftHandPose.gameObject.SetActive(false);
                leftHandModel_Geom.SetActive(true);
            
        }
        
        handData.animator.enabled = true;
    }







    public void SetLeftHandDataValues(handDataPose h1, handDataPose h2)
    {
        startingHandPosition_left = new Vector3(h1.root.localPosition.x / h1.root.localScale.x,
            h1.root.localPosition.y / h1.root.localScale.y, h1.root.localPosition.z / h1.root.localScale.z);
        finalHandPosition_left = new Vector3(h2.root.localPosition.x / h2.root.localScale.x,
            h2.root.localPosition.y / h2.root.localScale.y, h2.root.localPosition.z / h2.root.localScale.z);

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
    public void SendHandData(handDataPose h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
    }
    public IEnumerator SetHandDataRoutine(handDataPose h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation, Vector3 startingPosition, Quaternion startingRotation, Quaternion[] startingBonesRotation)
    {
        float timer = 0;
        while (timer < poseTransitionDuration)
        {
            Vector3 p = Vector3.Lerp(startingPosition, newPosition, timer / poseTransitionDuration);
            Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);
            h.root.localPosition = p;
            h.root.localRotation = r;
            for (int i = 0; i < newBonesRotation.Length; i++)
            {
                h.fingerBones[i].localRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i], timer / poseTransitionDuration);
            }



            timer += Time.deltaTime;
            yield return null;

        }


    }



}
