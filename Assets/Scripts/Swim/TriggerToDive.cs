using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TriggerToDive : MonoBehaviour
{
    /*private XRGrabInteractable grabInteractable;
    private bool isTriggered = false;
    public const string OnMaskGrabbed = "OnMaskGrabbed";*/
    
    private bool isCollided = false;
    public GameObject mask;/*抓取*/
    public GameObject mask1;/*眼前替代*/
    //public GameObject playerCamera;
    public GameObject oceanPlane;
    
    public float riseSpeed = 0.5f; // 上升速度
    public float targetHeight = 5f; // 目标高度
    public GrabHandPose poseGrab;
    void Start()
    {
        /*grabInteractable = GetComponent<XRGrabInteractable>();
        // 订阅 Select Enter 事件
        grabInteractable.selectEntered.AddListener(OnSelectEnter);*/
       // mask.SetActive(false);
        if (mask1 != null)
        {
            mask1.SetActive(false);
        }
        if(oceanPlane != null)
        {
            oceanPlane.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (poseGrab.HandGrabing == 0) return;

        if (other.gameObject==mask.gameObject)
        {
            isCollided = true;
            Debug.Log("Player camera collided with mask!");
            if (mask != null)
            {
                mask.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Mask is not assigned!");         
            }
            
            if (mask1 != null)
            {
                mask1.SetActive(true);
            }
            
            if (oceanPlane != null)
            {
                Debug.Log("Starting to raise ocean plane...");
                oceanPlane.SetActive(true); // 确保在开始时隐藏海洋平面
            }
            else
            {
                Debug.LogWarning("Ocean plane is not assigned!");
            }
        }
    }
    
    /*IEnumerator WaitAndRaiseOceanPlane()
    {
        yield return new WaitForSeconds(2f); // 等待2秒
        
        Vector3 startPosition = oceanPlane.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y + targetHeight, startPosition.z);
        if (oceanPlane != null)
        {
            /*oceanPlane.SetActive(true);#1#
            while (oceanPlane.transform.position.y < startPosition.y +targetHeight)
            {
                oceanPlane.transform.position = Vector3.MoveTowards(
                    oceanPlane.transform.position,
                    targetPosition,
                    riseSpeed * Time.deltaTime
                );
                yield return null; // 等待下一帧
            }
        }
    }*/
    /*private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("Photo grabbed!");

        if (!isTriggered)
        {
            isTriggered = true;

            EventManager.Instance.Trigger(OnMaskGrabbed,"MaskGrabbed");

        }
    }
    
    void OnDestroy()
    {
        // 取消订阅事件
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }*/
}
