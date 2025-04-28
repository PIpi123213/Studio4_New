using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowZipline : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 offset;

    void Start()
    {
        // �����ʼƫ����������������ڸ������λ��
        if (transform.parent != null)
            offset = transform.localPosition;
    }

    void LateUpdate()
    {
        // ����и����壬�����������λ��
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + offset;
        }
    }
}
