using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ZipLine : MonoBehaviour
{
    public Transform playerTransform; // ��� transform
    //public Transform startPoint;      // �������
    //public Transform endPoint;        // �����յ�
    public float speed = 5f;          // �����ٶ�
    public Transform zipLineHandler;
    public Vector3 sliderPlayerposition;
    public float rotationSpeed = 30f;      // ��ת�ٶȣ���/�룩
   // ������ (����������)
    public Transform[] waypoints;
    public CustomClimbInteractable grabInteractable;
    private IXRSelectInteractor playerInteractor;
    public static bool isSliding = false;
   
    private XRInteractionManager interactionManager;
    private InteractionLayerMask originalLayer;
    private Quaternion[] waypointRotations;
    public static bool isDone=false;
    private Vector3 initialPlayerOffset; // ��¼��Һͻ�����֮��ĳ�ʼƫ����
    public HandPoseSlider handPoseSlider;
    public CustomClimbProvider climbProvider;
    
    public AttachAnchor attachAnchor;
    private void Start()
    {
        PrecalculateWaypointRotations();
        interactionManager = grabInteractable.interactionManager;

        // ��¼ԭʼ������
        originalLayer = grabInteractable.interactionLayers;

        // ����ץȡ�¼�
        grabInteractable.selectEntered.AddListener(OnGrab);
        //grabInteractable.selectExited.AddListener(OnRelase);
        grabInteractable.selectExited.AddListener(UnSetPose);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(UnSetPose);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (isSliding||isDone) return;

        playerInteractor = args.interactorObject as IXRSelectInteractor;
        
     
        if (playerInteractor != null)
        {
            if (grabInteractable.interactorsSelecting.Count == 2 && !isDone)
            {
                isSliding = true;
                attachAnchor.ResetAnchorPoint();
                // **�����������**
                //grabInteractable.interactionLayers = 0; // �������н�����
                handPoseSlider.ziplineActive = true;
                // ��¼��Һͻ�����֮��ĳ�ʼƫ����
              

                // ��ʼ����
                StartCoroutine(SlideAlongZipline());
            }

          
        }
    }
    private void UnSetPose(SelectExitEventArgs args)
    {
        
        if (isSliding || isDone) return;

        playerInteractor = args.interactorObject as IXRSelectInteractor;
        if (playerInteractor != null)
        {
            if (grabInteractable.interactorsSelecting.Count == 0 )
            {
              
            }


        }



    }
    /*  private System.Collections.IEnumerator SlideAlongZipline()
      {
          // ���������ƶ������λ��
          zipLineHandler.position = startPoint.position;
          float currentSpeed = 0f; // ��ǰ�ٶȴ� 0 ��ʼ
          float acceleration = speed / 2.5f; 
          // �������й���
          while (zipLineHandler != null && Vector3.Distance(zipLineHandler.position, endPoint.position) > 0.1f)
          {
              currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
              // ���������ٶ����յ��ƶ�
              zipLineHandler.position = Vector3.MoveTowards(
             zipLineHandler.position, endPoint.position, currentSpeed * Time.deltaTime
              );

              // ��Ҹ��ݻ�������λ�ñ������λ��
              if (playerTransform != null)
              {
                  playerTransform.position = zipLineHandler.position + initialPlayerOffset;
              }

              yield return null;
          }

          EndZiplineRide();
      }*/
    void PrecalculateWaypointRotations()
    {
        waypointRotations = new Quaternion[waypoints.Length];
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector3 direction = (waypoints[i + 1].position - waypoints[i].position).normalized;
            // ��X��ָ��������������������
            direction.y = 0;
            waypointRotations[i] = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);
        }
        waypointRotations[waypoints.Length - 1] = waypointRotations[waypoints.Length - 2];
    }
    private System.Collections.IEnumerator SlideAlongZipline()
    {
        // ȷ��������������
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("��Ҫ��������2��·���㣡");
            yield break;
        }
        if (zipLineHandler != null && playerTransform != null)
        {
            initialPlayerOffset = playerTransform.position - zipLineHandler.position;
        }
        //����Ϊzipline������
        Transform originalParent = playerTransform.parent;
        playerTransform.SetParent(zipLineHandler);

        //ȡ�������Է�ֹλ��ƫ��
        grabInteractable.interactionLayers = 0;
        //StartCoroutine(setSlideringPlayerposition());
        



        // ��ʼ�����ƶ�����һ����
        zipLineHandler.position = waypoints[0].position;
        int currentWaypointIndex = 1;
        float currentSpeed = 0f;
        float acceleration = speed / 3f;

        // ��������·����
        while (currentWaypointIndex < waypoints.Length)
        {
            Transform start = waypoints[currentWaypointIndex - 1];
            Transform end = waypoints[currentWaypointIndex];
            Quaternion targetRotation = waypointRotations[currentWaypointIndex - 1];
            // ���λ����߼�
            while (Vector3.Distance(zipLineHandler.position, end.position) > 0.1f)
            {
                // �����߼�
                currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);

                // �ƶ�������
                zipLineHandler.position = Vector3.MoveTowards(
                    zipLineHandler.position,
                    end.position,
                    currentSpeed * Time.deltaTime
                );
            
          /*      zipLineHandler.rotation = Quaternion.RotateTowards(
                  zipLineHandler.rotation,
                  targetRotation,
                  rotationSpeed * Time.deltaTime
                );*/
                zipLineHandler.rotation = Quaternion.Slerp(
                    zipLineHandler.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );  

                // ͬ�����λ��
                /*  if (playerTransform != null)
                  {
                      playerTransform.position = zipLineHandler.position + initialPlayerOffset;
                      playerTransform.rotation = zipLineHandler.rotation * Quaternion.Euler(0, -180, 0);
                  }*/

                yield return null;
            }

            // ���ﵱǰ�յ���л�����һ��
            currentWaypointIndex++;
            //initialPlayerOffset = playerTransform.position - zipLineHandler.position;
            //currentSpeed = 0f; // �����ٶȣ���ѡ��
        }
        playerTransform.SetParent(originalParent);
        EndZiplineRide();
    }
    public IEnumerator setSlideringPlayerposition()
    {
       
       
        while (Vector3.Distance(playerTransform.localPosition, sliderPlayerposition) > 0.1f)
        {
            // �����߼�
            playerTransform.localPosition = Vector3.MoveTowards(
                  playerTransform.localPosition,
                  sliderPlayerposition,
                  0.1f * Time.deltaTime
              );
            yield return null;
        }


       


    }
    private void EndZiplineRide()
    {
        //isSliding = false;
        isDone = true;
        handPoseSlider.ziplineActive = false;
        //playerTransform.rotation = new Quaternion(0, playerTransform.rotation.y, 0, 0);

        if (handPoseSlider.isUnset)
        {
            handPoseSlider.ProcessPendingExitEvents();
        }
        else
        {
            var interactors = new List<IXRSelectInteractor>(grabInteractable.interactorsSelecting);
            foreach (var interactor in interactors)
            {
                interactionManager.SelectExit(interactor, grabInteractable);
            }
        }
        // **�ָ�����**
        grabInteractable.interactionLayers = originalLayer;
        //dynamicMoveProvider.useGravity = true;
        // **ǿ���ͷ����**
    }
}
