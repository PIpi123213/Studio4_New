using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCartonsController : MonoBehaviour
{
    public GameObject[] cartons; // 需要激活的纸箱数组

    private int currentIndex = 0; // 当前激活的纸箱索引

    public GameObject[] ArCass;
    private void Awake()
    {
        for (int i = 0; i < cartons.Length; i++)
        {
            cartons[i].SetActive(false);
        }
    }

    private void Start()
    {
        // 先全部隐藏
     
        
        EventManager.Instance.Subscribe(SignalReceiver.ToActiveCarton, ActivateCarton);
    }
    
    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe(SignalReceiver.ToActiveCarton, ActivateCarton);
        }
    }
    
    /*激活制定纸箱*/
    private void ActivateCarton(object obj)
    {
        Debug.Log("激活纸箱");
        // 检查当前索引是否在范围内
        if (currentIndex >= 0 && currentIndex < cartons.Length)
        {
            // 激活当前纸箱
            cartons[currentIndex].SetActive(true);
            currentIndex++;
        }
        else
        {
            Debug.LogWarning("No more cartons to activate.");
        }
    }
    public void Cassdiss()
    {
        for (int i = 0; i < ArCass.Length; i++)
        {
            ArCass[i].SetActive(false);
        }





    }
}
