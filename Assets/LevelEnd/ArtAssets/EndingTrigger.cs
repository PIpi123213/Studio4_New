using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public GameObject HospitalScene; // 医院场景

    public GameObject Door; // 玩家对象

    // Start is called before the first frame update
    void Start()
    {
        if (HospitalScene != null)
        {
            //HospitalScene.SetActive(false); // 初始时隐藏医院场景
        }
        else
        {
            Debug.LogWarning("HospitalScene GameObject is not assigned!");
        }

        if (Door != null)
        {
           // Door.SetActive(false); // 初始时隐藏门
        }
        else
        {
            Debug.LogWarning("Door GameObject is not assigned!");
        }
    }

    public void ActivateHospitalScene()
    {
        if (HospitalScene != null)
        {
            HospitalScene.SetActive(true); // 激活医院场景
        }
        else
        {
            Debug.LogWarning("HospitalScene GameObject is not assigned!");
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
