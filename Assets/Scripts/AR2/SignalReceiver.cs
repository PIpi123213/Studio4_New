using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SignalReceiver : MonoBehaviour
{
    /*private PlayableDirector director;

    private void Awake()
    {
        // 尝试找到同物体上的PlayableDirector组件
        director = GetComponent<PlayableDirector>();
        if (director == null)
        {
            Debug.LogWarning("TimelineSignalReceiver: 没有找到 PlayableDirector 组件！");
        }
    }*/
    
    public const string Photo1End = "Photo1End";
    public const string Photo2End = "Photo2End";
    public const string Photo3End = "Photo3End";
    public const string ToActiveCarton = "ToActiveCarton";

    public void OnInterationComplete()
    {
        EventManager.Instance.Trigger(ARInteractionManager.InteractionComplete,null);
    }

    public void TriggerEvent(string eventName)
    {
        // 触发事件
        EventManager.Instance.Trigger(eventName, null);
        /*
        Debug.Log("1111");
    */
    }
}
