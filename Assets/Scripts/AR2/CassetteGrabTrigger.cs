using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CassetteGrabTrigger : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool hasTriggered = false;
    
    public const string OnCassetteGrabbed = "OnCassetteGrabbed";
    
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 订阅 Select Enter 事件
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("Cassette grabbed!");

        if (!hasTriggered)
        {
            hasTriggered = true;
            // 触发事件
            EventManager.Instance.Trigger(OnCassetteGrabbed,"CassetteGrabbed");
        }
    }

    void OnDestroy()
    {
        // 取消订阅事件
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }
}
