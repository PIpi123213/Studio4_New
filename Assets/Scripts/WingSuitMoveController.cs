using System;
using UnityEngine;
using System.Collections;
using Unity.SharpZipLib.Zip.Compression.Streams;

public class WingSuitMoveController : MonoBehaviour
{
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private bool isRotatingAway = false; // 新增标志

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        //生成尾翼
        trailRenderer = gameObject.AddComponent<TrailRenderer>();
        trailRenderer.time       = 100.0f;
        trailRenderer.startWidth = 0.5f;
        trailRenderer.endWidth   = 0.1f;
        trailRenderer.material   = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.startColor = Color.white;
        trailRenderer.endColor   = Color.clear;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, -transform.forward * 10f, Color.red);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        if (!isRotatingAway) // 只有不处于纠正旋转时才响应输入
        {
            ApplyRotation();
        }
        Debug.Log("Rigidbody Velocity: " + rb.velocity);

    }

    [SerializeField] private float glideSpeed = 1000f;
    private void ApplyMovement()
    {
        if (isAddingUpwardVelocity) return; // 如果正在增加向上速度，跳过对 rb.velocity 的修改

        DetectDive();

        Vector3 glideVelocity = transform.forward * glideSpeed;
        glideVelocity.y = verticalSpeed;

        rb.velocity = glideVelocity;
    }

    // 仅用 yaw 控制水平旋转即可
    private                  float     yaw = 0f;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    private                  float     currentYaw  = 0f; // 实际旋转的 Y 值（带缓动）
    private                  float     yawVelocity = 0f; // 平滑用的速度缓存变量

    private void ApplyRotation()
    {
        if (isRotatingAway) return; // 如果正在旋转，跳过输入控制

        yaw        += (leftController.position.y - rightController.position.y) * 30f * Time.deltaTime;
        currentYaw =  Mathf.SmoothDampAngle(currentYaw, yaw, ref yawVelocity, 1f);

        Quaternion targetRotation = Quaternion.Euler(0f, currentYaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    //碰撞物体检测以及转向
    private void OnTriggerEnter(Collider other)//检测
    {
        Debug.Log("Detected object: " + other.name);
        // 获取碰撞点和法线
        Vector3 contactPoint       = other.ClosestPoint(transform.position);
        Vector3 directionToContact = (contactPoint - transform.position).normalized;
        // 判断法线方向
        if (Mathf.Abs(directionToContact.y) > 0.7f) // 接近水平
        {
            Debug.Log("Horizontal object detected, lifting...");
            StartCoroutine(GraduallyAddUpwardVelocity(glideSpeed, 3f)); // 目标速度增量 10f，持续时间 1f
        }
        else // 接近竖直
        {
            Debug.Log("Vertical object detected, rotating...");
            Vector3 directionAway = transform.forward * 1f + (transform.position - contactPoint).normalized;
            StartCoroutine(SmoothRotateAway(directionAway, 2f));
        }

    }

    [SerializeField] private float     defaultVerticalSpeed;
    [SerializeField] private float     gravityFactor = 10f; //控制俯冲比例
    private                  float     verticalSpeed ;
    [SerializeField] private Transform Head;
    private void DetectDive()
    {
        float averageHeight = (Head.position.y) - ((leftController.position.y + rightController.position.y) / 2f);
        if(averageHeight >= 0f)
        {
            verticalSpeed = defaultVerticalSpeed - averageHeight * gravityFactor;

        }
        else
        {
            verticalSpeed = Mathf.Lerp(defaultVerticalSpeed, defaultVerticalSpeed * 0.5f, Mathf.Abs(averageHeight) / 0.5f) - averageHeight * gravityFactor; // 俯冲时，速度会减小
        }

        Debug.Log("Vertical Speed: " + verticalSpeed);

    }

    // 触发器检测到地面后，抬起玩家
    private bool isAddingUpwardVelocity = false; // 标志位

    private IEnumerator GraduallyAddUpwardVelocity(float targetUpwardSpeed, float duration)
    {
        isAddingUpwardVelocity = true; // 设置标志位

        float elapsedTime          = 0f;
        float halfDuration         = duration / 2f; // 分为两段时间
        float initialVerticalSpeed = rb.velocity.y;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // 根据时间分段处理速度插值
            float smoothT;
            if (elapsedTime <= halfDuration)
            {
                // 第一阶段：速度渐强
                smoothT = Mathf.SmoothStep(0f, 1f, elapsedTime / halfDuration);
            }
            else
            {
                // 第二阶段：速度渐弱
                smoothT = Mathf.SmoothStep(1f, 0f, (elapsedTime - halfDuration) / halfDuration);
            }

            Vector3 currentVelocity = rb.velocity;
            if (elapsedTime <= halfDuration)
            {
                currentVelocity.y = Mathf.Lerp(initialVerticalSpeed, initialVerticalSpeed + targetUpwardSpeed, smoothT);
            }
            else
            {
                currentVelocity.y = Mathf.Lerp(initialVerticalSpeed + targetUpwardSpeed, initialVerticalSpeed, smoothT);
            }
            rb.velocity = currentVelocity;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isAddingUpwardVelocity = false; // 恢复标志位
    }

    //水平转向
    private IEnumerator SmoothRotateAway(Vector3 direction, float duration)
    {
        isRotatingAway = true;

        Quaternion initialRotation = rb.rotation;

        // 仅计算目标方向的 yaw 旋转
        Vector3    targetDirection = new Vector3(direction.x, 0f, direction.z).normalized;
        Quaternion targetRotation  = Quaternion.LookRotation(targetDirection);

        // 保持当前的 pitch 和 roll
        targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t       = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Quaternion newRotation = Quaternion.Slerp(initialRotation, targetRotation, smoothT);

            rb.MoveRotation(newRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MoveRotation(targetRotation);

        // 确保最终角度更新
        yaw         = targetRotation.eulerAngles.y;
        currentYaw  = yaw;
        yawVelocity = 0f;

        isRotatingAway = false;
    }

}