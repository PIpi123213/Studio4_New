using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
[CreateAssetMenu(menuName = "XR/Physical Movement Data")]
public class MoveInSpace:ScriptableObject
{
    // ���ò���
    [Header("Settings")]
    [SerializeField] private bool _ignoreVertical = true;

    // ����ʱ����
    [System.NonSerialized] private Vector3 _currentOffset;

#if UNITY_EDITOR
    [Header("Debug View")]
    [SerializeField] private Vector3 _debugOffset;
#endif

    public Vector3 Offset => _currentOffset;

    public void InitializeSystem()
    {
        _currentOffset = Vector3.zero;
#if UNITY_EDITOR
        _debugOffset = _currentOffset;
#endif
        Debug.Log("����ϵͳ�ѳ�ʼ�� (ԭ��: 0,0,0)");
    }

    public void UpdateOffset(Vector3 newOffset)
    {
        _currentOffset = _ignoreVertical ?
            new Vector3(newOffset.x, 0, newOffset.z) :
            newOffset;

#if UNITY_EDITOR
        _debugOffset = _currentOffset;
#endif
    }
}
