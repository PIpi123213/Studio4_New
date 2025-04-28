using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public static DontDestoryPlayer Instance { get; private set; }

    private void Awake()
    {
        // ˫���пձ���
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // ������ʵ��
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // ��������Ϊ gameObject
    }

}
