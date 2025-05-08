using System;
using UnityEngine;
using System.Collections;
using Unity.SharpZipLib.Zip.Compression.Streams;

public class WingSuitMoveController : MonoBehaviour
{
    private Rigidbody     rb;
    private TrailRenderer trailRenderer;
    private bool          isRotatingAway = false; // 新增标志

    void Start()
    {
        rb            = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // 同步 Rigidbody 的初始旋转
        rb.rotation = transform.rotation;

        currentYaw = transform.eulerAngles.y; // 初始化 currentYaw
        yaw        = currentYaw;

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
        ApplyMovement(); // 始终允许玩家移动

        if (!isForcedToSkull) // 如果未强制飞向 Skull，允许玩家控制转向
        {
            ApplyRotation();
        }

        LimitPlayerHeight();
        CheckAndLiftIfGroundBelow();
    }

    [SerializeField] private float glideSpeed = 1000f;

    private void ApplyMovement()
    {
        // if (isAddingUpwardVelocity) return; // 如果正在增加向上速度，跳过对 rb.velocity 的修改

        DetectDive();

        Vector3 glideVelocity = transform.forward * glideSpeed;
        glideVelocity.y = verticalSpeed;

        rb.velocity = glideVelocity;
    }

    // 仅用 yaw 控制水平旋转即可
    private                  float     yaw = 0f;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    private                  float     currentYaw;       // 实际旋转的 Y 值（带缓动）
    private                  float     yawVelocity = 0f; // 平滑用的速度缓存变量

    private void ApplyRotation()
    {
        if (isRotatingAway) return; // 如果正在旋转，跳过输入控制

        yaw        += (leftController.position.y - rightController.position.y) * 3f;
        currentYaw =  Mathf.Lerp(currentYaw, yaw, 1f);

        Quaternion targetRotation = Quaternion.Euler(0f, currentYaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    private void CheckSideRaycasts()
    {
        float rayLength = 20f;

        // 向左发射射线
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit leftHit, rayLength))
        {
            if (leftHit.collider.CompareTag("Wall")) // 检查是否为 Wall
            {
                Debug.Log("Left Wall detected: " + leftHit.collider.name);
                StartCoroutine(MoveSideways(-transform.right, 30f, 1f)); // 向右移动
            }
        }

        // 向右发射射线
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit rightHit, rayLength))
        {
            if (rightHit.collider.CompareTag("Wall")) // 检查是否为 Wall
            {
                Debug.Log("Right Wall detected: " + rightHit.collider.name);
                StartCoroutine(MoveSideways(transform.right, 30f, 1f)); // 向左移动
            }
        }
    }


    private IEnumerator MoveSideways(Vector3 direction, float distance, float duration)
    {
        Vector3 startPosition  = transform.position;
        Vector3 targetPosition = startPosition + direction * distance;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t       = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 保留当前的前进速度
            Vector3 forwardVelocity = rb.velocity.z * transform.forward;

            // 计算侧向移动的位置
            Vector3 sidewaysPosition = Vector3.Lerp(startPosition, targetPosition, smoothT);

            // 使用 Rigidbody 的 MovePosition
            rb.MovePosition(sidewaysPosition + forwardVelocity * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置正确
        rb.MovePosition(targetPosition + rb.velocity.z * transform.forward * Time.deltaTime);
    }

    // private void HandleSideCollision(RaycastHit hit)
    // {
    //     Vector3 contactPoint  = hit.point;
    //     Vector3 directionAway = (transform.position - contactPoint).normalized + transform.forward;
    //
    //     // 执行旋转逻辑
    //     StartCoroutine(SmoothRotateAway(directionAway, 1f));
    // }

    //碰撞物体检测以及转向
    private                  bool      isForcedToSkull = false; // 标志位，表示是否强制飞向 Skull
    [SerializeField] private Transform skullTransform;          // Skull 的 Transform
    public const string OnObstacleDetected = "OnObstacleDetected"; // 事件名称
    private void OnTriggerEnter(Collider other)                 //检测
    {
        Debug.Log("Detected object: " + other.name);
        // 获取碰撞点和法线
        Vector3 contactPoint       = other.ClosestPoint(transform.position);
        Vector3 directionToContact = (contactPoint - transform.position).normalized;
        // 判断法线方向
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Vertical object detected, rotating parallel to wall...");
            Vector3 wallNormal = other.ClosestPoint(transform.position) - transform.position;
            StartCoroutine(SmoothRotateParallelToWall(wallNormal, 1f));
            // EventManager.Instance.Trigger(OnObstacleDetected, "air_leaking"); // 触发事件
        }
        else if (other.CompareTag("pillar"))
        {
            // 计算物体相对于玩家的位置
            Vector3 directionToObject = other.transform.position - transform.position;
            float   dotProduct        = Vector3.Dot(transform.right, directionToObject);
            if (Vector3.Dot(transform.forward, directionToObject) < 0)
            {
                if (dotProduct > 0) // 物体在玩家右边
                {
                    Debug.Log("Object detected on the right, rotating to left-forward...");
                    Vector3 targetDirection = (transform.forward - transform.right).normalized; // 左前方
                    StartCoroutine(SmoothRotateToDirection(targetDirection, 1f));
                }
                else if (dotProduct < 0) // 物体在玩家左边
                {
                    Debug.Log("Object detected on the left, rotating to right-forward...");
                    Vector3 targetDirection = (transform.forward + transform.right).normalized; // 右前方
                    StartCoroutine(SmoothRotateToDirection(targetDirection, 1f));
                }
            }
        }
        else if (other.name == "RushToDeathArea")
        {
            Debug.Log("Entering RushToDeathArea, forcing player to fly towards Skull...");
            isForcedToSkull = true; // 禁用玩家转向控制

            // 计算目标方向
            Vector3 directionToSkull = (skullTransform.position - transform.position).normalized;

            // 启动协程，平滑旋转到目标方向
            StartCoroutine(SmoothRotateToDirection(directionToSkull, 2f));
        }
    }


    private IEnumerator SmoothRotateToDirection(Vector3 targetDirection, float duration)
    {
        isRotatingAway = true; // 暂停 ApplyRotation

        Quaternion initialRotation = rb.rotation;
        Quaternion targetRotation  = Quaternion.LookRotation(targetDirection);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t       = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 插值旋转
            Quaternion newRotation = Quaternion.Slerp(initialRotation, targetRotation, smoothT);
            rb.MoveRotation(newRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终角度更新
        rb.MoveRotation(targetRotation);

        // 更新 yaw 和 currentYaw
        yaw         = targetRotation.eulerAngles.y;
        currentYaw  = yaw;
        yawVelocity = 0f;

        isRotatingAway = false; // 恢复 ApplyRotation
    }

    [SerializeField] private float     defaultVerticalSpeed;
    [SerializeField] private float     gravityFactor = 10f; //控制俯冲比例
    private                  float     verticalSpeed;
    [SerializeField] private Transform Head;

    private void DetectDive()
    {
        float averageHeight = (Head.position.y) - ((leftController.position.y + rightController.position.y) / 2f);
        if (averageHeight >= 0f)
        {
            verticalSpeed = defaultVerticalSpeed - averageHeight * gravityFactor;

        }
        else
        {
            verticalSpeed =
                Mathf.Lerp(defaultVerticalSpeed, defaultVerticalSpeed * 0.5f, Mathf.Abs(averageHeight) / 0.5f) -
                averageHeight * gravityFactor; // 俯冲时，速度会减小
        }
    }

    private void CheckAndLiftIfGroundBelow()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                StartCoroutine(GraduallyAddAndReduceUpwardVelocity(30f, 2f));
            }
        }
    }

    // 触发器检测到地面后，抬起玩家
    private bool isAddingUpwardVelocity = false; // 标志位

    private void LimitPlayerHeight()
    {
        float      maxDistanceFromGround = 70f; // 最大离地距离
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

    private IEnumerator GraduallyAddAndReduceUpwardVelocity(float maxUpwardSpeed, float duration)
    {
        isAddingUpwardVelocity = true; // 设置标志位

        float halfDuration = duration / 2f; // 加速和减速各占一半时间
        float elapsedTime  = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime < halfDuration
                ? elapsedTime / halfDuration                   // 前半段加速
                : (elapsedTime - halfDuration) / halfDuration; // 后半段减速

            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float upwardVelocity = elapsedTime < halfDuration
                ? Mathf.Lerp(1f, maxUpwardSpeed, smoothT)  // 从1加速到maxUpwardSpeed
                : Mathf.Lerp(maxUpwardSpeed, 1f, smoothT); // 从maxUpwardSpeed减速到1

            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = upwardVelocity;
            rb.velocity       = currentVelocity;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAddingUpwardVelocity = false; // 恢复标志位
    }

    //水平转向
    private IEnumerator SmoothRotateParallelToWall(Vector3 wallNormal, float duration)
    {
        isRotatingAway = true; // 暂停 ApplyRotation

        Quaternion initialRotation = rb.rotation;

        // 计算与墙平行的方向
        Vector3 parallelDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        // 确保方向与当前前进方向一致
        if (Vector3.Dot(parallelDirection, transform.forward) < 0)
        {
            parallelDirection = -parallelDirection;
        }

        Quaternion targetRotation = Quaternion.LookRotation(parallelDirection);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t       = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 插值旋转
            Quaternion newRotation = Quaternion.Slerp(initialRotation, targetRotation, smoothT);
            rb.MoveRotation(newRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终角度更新
        rb.MoveRotation(targetRotation);

        // 更新 yaw 和 currentYaw
        yaw         = targetRotation.eulerAngles.y;
        currentYaw  = yaw;
        yawVelocity = 0f;

        isRotatingAway = false; // 恢复 ApplyRotation
    }
}