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
    public GameObject playerCamera;
    
    void Start()
    {
        /*grabInteractable = GetComponent<XRGrabInteractable>();
        // 订阅 Select Enter 事件
        grabInteractable.selectEntered.AddListener(OnSelectEnter);*/
        
        if (mask1 != null)
        {
            mask1.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject==playerCamera.gameObject)
        {
            isCollided = true;
            Debug.Log("Player camera collided with mask!");
            if (mask != null)
            {
                mask.SetActive(false);
            }
            
            if (mask1 != null)
            {
                mask1.SetActive(true);
            }
        }
    }
    
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
