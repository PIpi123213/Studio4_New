using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followChar : MonoBehaviour
{

    public Transform target;// 另一个物体


    public Vector3 positionOffset = Vector3.zero;

    private Vector3 initialLocalPosition;
    private Quaternion initialRotationDifference;
    void Start()
    {

        Vector3 localOffset = target.InverseTransformPoint(transform.position);
        initialLocalPosition = new Vector3(localOffset.x, 0, localOffset.z);

        // 记录初始旋转差异
        initialRotationDifference = Quaternion.Inverse(target.rotation) * transform.rotation;
    }

    
    // 示例：按空格键切换
    void Update()
    {
        // 计算目标的 Y 轴旋转
        float targetYRotation = target.eulerAngles.y;

        // 创建仅包含 Y 轴旋转的四元数
        Quaternion rotationY = Quaternion.Euler(0, targetYRotation, 0);

        // 计算新的位置（保持 Y 轴不变）
        Vector3 newPosition = target.position + rotationY * (initialLocalPosition + positionOffset);
        newPosition.y = transform.position.y; // 保持原始 Y 轴位置

        // 应用新的位置
        transform.position = newPosition;

        // 应用初始旋转差异
        transform.rotation = rotationY * initialRotationDifference;

        // 更新位置
    }


    void LateUpdate()
    {
        
        
          

    }
}
