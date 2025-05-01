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

        // 同步 Rigidbody 的初始旋转
        rb.rotation = transform.rotation;

        //生成尾翼
        // trailRenderer = gameObject.AddComponent<TrailRenderer>();
        // trailRenderer.time       = 100.0f;
        // trailRenderer.startWidth = 0.5f;
        // trailRenderer.endWidth   = 0.1f;
        // trailRenderer.material   = new Material(Shader.Find("Sprites/Default"));
        // trailRenderer.startColor = Color.white;
        // trailRenderer.endColor   = Color.clear;
    }

 

    private void Update()
    {
        ApplyMovement();
        if (!isRotatingAway) // 只有不处于纠正旋转时才响应输入
        {
            ApplyRotation();
        }

        LimitPlayerHeight();

        Debug.Log("Rigidbody Velocity: " + rb.velocity);

    }

    [SerializeField] private float glideSpeed = 1000f;
    private void ApplyMovement()
    {
        if (isAddingUpwardVelocity) return; // 如果正在增加向上速度，跳过对 rb.velocity 的修改

        DetectDive();

        // 根据 rb.velocity.y 动态调整 glideSpeed
        // glideSpeed = Mathf.Lerp(500f, 2000f, Mathf.Clamp01(rb.velocity.y / 10f)); // 这里假设 rb.velocity.y 在 0 到 10 之间变化

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

        yaw        += (leftController.position.y - rightController.position.y) * 70f * Time.deltaTime;
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
            Vector3 directionAway = transform.forward * 2f + (transform.position - contactPoint).normalized;
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

    private void LimitPlayerHeight()
    {
        float      maxDistanceFromGround = 30f; // 最大离地距离
        RaycastHit hit;

        // 从玩家位置向下发射射线
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float distanceFromGround = hit.distance;

            if (distanceFromGround > maxDistanceFromGround)
            {
                // 如果超过最大距离，强制调整垂直速度向下
                Vector3 currentVelocity = rb.velocity;
                currentVelocity.y = Mathf.Min(currentVelocity.y, -1f); // 确保速度向下
                rb.velocity       = currentVelocity;
            }
        }
    }
    private IEnumerator GraduallyAddUpwardVelocity(float additionalUpwardSpeed, float duration)
    {
        isAddingUpwardVelocity = true; // 设置标志位

        float elapsedTime          = 0f;
        float initialVerticalSpeed = rb.velocity.y; // 获取当前的垂直速度

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // 使用平滑插值增加向上的速度
            float smoothT        = Mathf.SmoothStep(0f, 1f, t);
            float upwardVelocity = Mathf.Lerp(0f, additionalUpwardSpeed, smoothT);

            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = initialVerticalSpeed + upwardVelocity;
            rb.velocity       = currentVelocity;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 平滑过渡到正常的垂直速度
        float transitionDuration = 1f; // 过渡时间
        float transitionElapsed  = 0f;
        float finalVerticalSpeed = defaultVerticalSpeed; // 目标垂直速度

        while (transitionElapsed < transitionDuration)
        {
            float t = transitionElapsed / transitionDuration;

            // 平滑插值到目标垂直速度
            float smoothT       = Mathf.SmoothStep(0f, 1f, t);
            float verticalSpeed = Mathf.Lerp(rb.velocity.y, finalVerticalSpeed, smoothT);

            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = verticalSpeed;
            rb.velocity       = currentVelocity;

            transitionElapsed += Time.deltaTime;
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