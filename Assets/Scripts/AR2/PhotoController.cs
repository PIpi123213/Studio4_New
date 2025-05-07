using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoController : MonoBehaviour
{
    public GameObject photo1;
    public Material photo1NewMaterial;

    public GameObject photo2;
    public Material photo2NewMaterial;
    
    public GameObject photo3;
    public Material photo3NewMaterial;
    
    private void Start()
    {
        EventManager.Instance.Subscribe(SignalReceiver.Photo1End, OnPhoto1End);
        EventManager.Instance.Subscribe(SignalReceiver.Photo2End, OnPhoto2End);
        EventManager.Instance.Subscribe(SignalReceiver.Photo3End, OnPhoto3End);
    }
    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(SignalReceiver.Photo1End, OnPhoto1End);
        EventManager.Instance.Unsubscribe(SignalReceiver.Photo2End, OnPhoto2End);
        EventManager.Instance.Unsubscribe(SignalReceiver.Photo3End, OnPhoto3End);
    }
    
    private void OnPhoto1End(object param)
    {
        // 检查 photo1 是否存在
        if (photo1 != null)
        {
            // 获取 photo1 的 Renderer 组件
            Renderer renderer = photo1.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 更改材质
                renderer.material = photo1NewMaterial;
            }
            else
            {
                Debug.LogWarning("Photo1 does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Photo1 is not assigned.");
        }
    }
    private void OnPhoto2End(object param)
    {
        // 检查 photo2 是否存在
        if (photo2 != null)
        {
            // 获取 photo2 的 Renderer 组件
            Renderer renderer = photo2.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 更改材质
                renderer.material = photo2NewMaterial;
            }
            else
            {
                Debug.LogWarning("Photo2 does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Photo2 is not assigned.");
        }
    }
    private void OnPhoto3End(object param)
    {
        // 检查 photo3 是否存在
        if (photo3 != null)
        {
            // 获取 photo3 的 Renderer 组件
            Renderer renderer = photo3.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 更改材质
                renderer.material = photo3NewMaterial;
            }
            else
            {
                Debug.LogWarning("Photo3 does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Photo3 is not assigned.");
        }
    }
}
