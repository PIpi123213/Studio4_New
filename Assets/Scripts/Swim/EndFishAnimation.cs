using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndFishAnimation : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform playerTran;  // 目标物体（如玩家）
    public Vector3 offset;        // 相对偏移量
    public float moveSpeed = 5f;  // 移动速度
    public float rotateSpeed = 180f; // 旋转速度（度/秒）
    public float arrivalDistance = 0.1f; // 判定到达目标的距离阈值
    public PlayableDirector endTimeline; // 要播放的Timeline

    private bool isStart = false;
    private bool hasArrived = false; // 是否已到达目标位置

    private void Update()
    {
        if (playerTran == null)
        {
            Debug.LogWarning("Player Transform 未赋值！");
            return;
        }

        if (isStart && !hasArrived)
        {
            Vector3 targetPosition = playerTran.position + offset;

            // 计算与目标位置的距离
            float distance = Vector3.Distance(transform.position, targetPosition);

            // 如果还未到达目标位置
            if (distance > arrivalDistance)
            {
                // 以 moveSpeed 速度向目标位置移动
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                // 计算朝向 playerTran 的方向（忽略 Y 轴，仅在 XZ 平面旋转）
                Vector3 directionToPlayer = playerTran.position - transform.position;
                directionToPlayer.y = 0; // 保持 Y 轴不变，仅旋转 Z 轴

                // 如果方向有效（非零），计算目标旋转角度
                if (directionToPlayer != Vector3.zero)
                {
                    // 计算目标旋转（Z 轴朝向 playerTran）
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

                    // 以 rotateSpeed 速度平滑旋转到目标角度
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime
                    );
                }
            }
            else
            {
                // 到达目标位置
                hasArrived = true;
                PlayEndTimeline();
            }
        }
    }

    public void StartAnimation()
    {
        isStart = true;
        hasArrived = false;
        Debug.Log("开始移动和旋转！");
    }

    private void PlayEndTimeline()
    {
        if (endTimeline != null)
        {
            Debug.Log("已到达目标位置，播放Timeline");
            endTimeline.Play();
        }
        else
        {
            Debug.LogWarning("未分配End Timeline！");
        }
    }
}
