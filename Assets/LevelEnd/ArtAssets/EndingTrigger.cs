using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public GameObject HospitalScene; // 医院场景
    public GameObject lightBeam; // 光束对象
    public GameObject hardDrive; // 硬盘对象
    public GameObject Door; // 玩家对象

    public GameObject UI1; // UI1对象
    public GameObject UI2; // UI2对象
    // Start is called before the first frame update
    void Start()
    {
        /*if (HospitalScene != null)
        {
            HospitalScene.SetActive(false); // 初始时隐藏医院场景
        }
        else
        {
            Debug.LogWarning("HospitalScene GameObject is not assigned!");
        }*/

        if (lightBeam != null)
        {
            lightBeam.SetActive(false); // 初始时隐藏光束
        }
        else
        {
            Debug.LogWarning("LightBeam GameObject is not assigned!");
        }
        
        if (hardDrive != null)
        {
            hardDrive.SetActive(false); // 初始时隐藏硬盘
        }
        else
        {
            Debug.LogWarning("HardDrive GameObject is not assigned!");
        }
        
        UI1.SetActive(false); // 初始时隐藏UI1
        UI2.SetActive(false); // 初始时隐藏UI2
        /*if (Door != null)
        {
           Door.SetActive(false); // 初始时隐藏门
        }
        else
        {
            Debug.LogWarning("Door GameObject is not assigned!");
        }*/
    }

    /*public void ActivateHospitalScene()
    {
        if (HospitalScene != null)
        {
            HospitalScene.SetActive(true); // 激活医院场景
        }
        else
        {
            Debug.LogWarning("HospitalScene GameObject is not assigned!");
        }
    }*/
    public void ActivateUI1()
    {
        if (UI1 != null)
        {
            UI1.SetActive(true); // 激活UI1
        }
        else
        {
            Debug.LogWarning("UI1 GameObject is not assigned!");
        }
    }
    public void ActivateUI2()
    {
        if (UI2 != null)
        {
            UI2.SetActive(true); // 激活UI2
        }
        else
        {
            Debug.LogWarning("UI2 GameObject is not assigned!");
        }
    }
    
    public void ActivateLightBeamAndHardDrive()
    {
        if (lightBeam != null)
        {
            lightBeam.SetActive(true); // 激活光束
            hardDrive.SetActive(true); 
        }
        else
        {
            Debug.LogWarning("LightBeam GameObject is not assigned!");
        }
    }

    
    public void ActivateDoor()
    {
        if (Door != null)
        {
            Door.SetActive(true); // 激活门
        }
        else
        {
            Debug.LogWarning("Door GameObject is not assigned!");
        }
    }
}
