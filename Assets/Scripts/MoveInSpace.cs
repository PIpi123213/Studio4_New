using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
[CreateAssetMenu(menuName = "XR/Physical Movement Data")]
public class MoveInSpace:ScriptableObject
{
    // 配置参数
    [Header("Settings")]
    [SerializeField] private bool _ignoreVertical = true;

    // 运行时数据
    [System.NonSerialized] private Vector3 _currentOffset;
    [System.NonSerialized] private Quaternion _currentRotation;

#if UNITY_EDITOR
    [Header("Debug View")]
    [SerializeField] private Vector3 _debugOffset;
    [SerializeField] private Quaternion _debugrotation;
#endif

    public Vector3 Offset => _currentOffset;
    public Quaternion Rotation => _currentRotation;
    public void InitializeSystem()
    {
        _currentOffset = Vector3.zero;
        _currentRotation = Quaternion.identity;
#if UNITY_EDITOR
        _debugOffset = _currentOffset;
        _debugrotation = _currentRotation;
#endif


        Debug.Log("坐标系统已初始化 (原点: 0,0,0)");
    }

    public void UpdateOffset(Vector3 newOffset,Quaternion newRotation)
    {
        if (_ignoreVertical)
        {
            _currentOffset = new Vector3(newOffset.x, 0, newOffset.z);
        }
        else
        {
            _currentOffset = newOffset;
        }
        _currentRotation = newRotation;


#if UNITY_EDITOR
        _debugOffset = _currentOffset;
        _debugrotation = _currentRotation;
#endif
    }
}
