using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class WingSuitMoveController : MonoBehaviour
{
    #region 组件引用
    private Rigidbody rb;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    [SerializeField] private Transform trackingSpace;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip air_leaking;
    [SerializeField] private AudioClip dead;
    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] private Transform skullTransform;

    [SerializeField] private GameObject SpeedLine;
    [SerializeField] private GameObject AudioManager;
    //[SerializeField] private PlayerStateTran playerStateTran; // 添加玩家状态引用
    #endregion

    #region 移动参数
    [Header("移动参数")]
    [SerializeField] private float glideSpeed = 1000f;
    [SerializeField] private float defaultVerticalSpeed = 0f;
    [SerializeField] private float gravityFactor = 10f;
    [SerializeField] private float maxTiltAngle = 30f;
    [SerializeField] private float rotationSensitivity = 1f;
    [SerializeField] private float maxHeightFromGround = 70f;
    [SerializeField] private float speedUpMultiplier = 1.5f;  // 加速倍数
    [SerializeField] private float speedUpGravityFactor = 1.5f;  // 加速时的重力倍数
    [SerializeField] private float pillarSideBoostForce = 500f;  // 柱子碰撞后的横向推力
    [SerializeField] private float pillarBoostDuration = 1f;  // 柱子碰撞后的加速持续时间
    [SerializeField] private Transform Head;  // 基准高度
    [SerializeField] private float tiltDeadZone = 0.05f; // 倾斜死区阈值
    [SerializeField] private float initialAccelerationTime = 3f; // 初始加速时间
    #endregion

    #region 私有变量
    private float verticalSpeed = 0f;
    private float yaw = 0f;
    private float currentYaw;
    private float yawVelocity = 0f;
    private bool isRotatingAway = false;
    private bool isForcedToSkull = false;
    private bool isAddingUpwardVelocity = false;
    private bool isInSpeedUpZone = false;  // 是否在加速区域
    private float originalGlideSpeed;  // 原始滑行速度
    private float originalGravityFactor;  // 原始重力因子
    private Vector3 pillarBoostDirection;  // 柱子碰撞后的推力方向
    private float pillarBoostTimer = 0f;  // 柱子碰撞后的加速计时器
    private float accelerationTimer = 0f; // 初始加速计时器
    private bool isInitialAcceleration = true;
    #endregion

    #region Unity生命周期
    private void Start()
    {
        InitializeComponents();
        SubscribeToEvents();
        originalGlideSpeed = glideSpeed;
        originalGravityFactor = gravityFactor;
        AudioManager.SetActive(false);


        // 添加碰撞检测设置
        if (rb != null)
        {
            // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            // rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void Update()
    {
        if (!PlayerStateTran.Instance.isStart) return;
        if (PlayerStateTran.Instance.Stage == 1)
        {
            AudioManager.SetActive(true);
        }
        else
        {
            AudioManager.SetActive(false);
        }

        HandleMovement();
        UpdatePillarBoost();
    }
    void FixedUpdate()
    {
        // Vector3 move = rb.velocity * Time.fixedDeltaTime;
        // if (Physics.Raycast(transform.position, move.normalized, out RaycastHit hit, move.magnitude + 1f))
        // {
        //     if (hit.collider.CompareTag("DeadEnd") || hit.collider.CompareTag("SpeedUp") || hit.collider.name == "RushToDeathArea")
        //     {
        //         return;
        //     }
        //     // 撞上了什么，处理碰撞（可以改为击退、停止、播放动画等）
        //     Debug.Log("即将穿模撞击: " + hit.collider.name);
        //     rb.velocity = Vector3.zero;
        // }
    }

    #endregion

    #region 初始化
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.rotation = transform.rotation;

        currentYaw = transform.eulerAngles.y;
        yaw = currentYaw;
    }

    private void SubscribeToEvents()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnPlayableDirectorStopped;
        }
    }
    #endregion

    #region 移动控制
    private void HandleMovement()
    {
        ApplyMovement();

        if (!isForcedToSkull)
        {
            ApplyRotation();
        }

        LimitPlayerHeight();
        CheckAndLiftIfGroundBelow();
    }

    private void ApplyMovement()
    {
        DetectDive();
        // 处理初始加速
        if (isInitialAcceleration)
        {
            accelerationTimer += Time.deltaTime;
            float t = Mathf.Clamp01(accelerationTimer / initialAccelerationTime);
            float currentSpeed = Mathf.Lerp(0, glideSpeed, t);

            Vector3 glideVelocity = transform.forward * currentSpeed;
            glideVelocity.y = verticalSpeed;
            rb.velocity = glideVelocity;

            if (accelerationTimer >= initialAccelerationTime)
            {
                isInitialAcceleration = false;
            }
        }
        else
        {
            Vector3 glideVelocity = transform.forward * glideSpeed;
            glideVelocity.y = verticalSpeed;
            rb.velocity = glideVelocity;
        }
    }

    private void ApplyRotation()
    {
        if (isRotatingAway) return;

        // 计算左右手高度差和平均高度
        float heightDifference = leftController.position.y - rightController.position.y;
        float averageHeight = (leftController.position.y + rightController.position.y) * 0.04f;

        // 死区处理：如果高度差绝对值小于阈值，则不倾斜
        float tiltAngle = 0f;
        if (Mathf.Abs(heightDifference) > tiltDeadZone)
        {
            tiltAngle = CalculateTiltAngle(heightDifference);
        }

        // 更新偏航角
        UpdateYaw(heightDifference);

        // 计算俯仰角度
        float pitchAngle = CalculatePitchAngle(averageHeight);

        // 应用旋转
        /*
        Debug.Log("tiltAngle: " + tiltAngle + " pitchAngle: " + pitchAngle);
        */
        ApplyRotations(-0.5f * tiltAngle, -pitchAngle);
    }

    private void UpdateYaw(float heightDifference)
    {
        yaw += heightDifference * rotationSensitivity;
        currentYaw = Mathf.Lerp(currentYaw, yaw, 1f);
    }

    private float CalculateTiltAngle(float heightDifference)
    {
        float tiltAngle = -heightDifference * maxTiltAngle;
        return Mathf.Clamp(tiltAngle, -maxTiltAngle, maxTiltAngle);
    }

    private float CalculatePitchAngle(float averageHeight)
    {
        // 将Head的位置转换到trackingSpace的局部坐标系中
        Vector3 headLocalPos = trackingSpace.InverseTransformPoint(Head.position);
        // 将控制器位置转换到trackingSpace的局部坐标系中
        Vector3 leftLocalPos = trackingSpace.InverseTransformPoint(leftController.position);
        Vector3 rightLocalPos = trackingSpace.InverseTransformPoint(rightController.position);
        float averageLocalHeight = (leftLocalPos.y + rightLocalPos.y) * 0.5f;

        /*Debug.Log("Head Local Y: " + headLocalPos.y);
        Debug.Log("Average Controller Local Y: " + averageLocalHeight);
        Debug.Log("Height Difference: " + (averageLocalHeight - headLocalPos.y));*/

        float heightScaleFactor = 0.5f;  // 缩放因子
        float pitchAngle = (averageLocalHeight - headLocalPos.y) * heightScaleFactor * maxTiltAngle;
        Debug.Log("Final Pitch Angle: " + pitchAngle);

        return Mathf.Clamp(pitchAngle, -maxTiltAngle, maxTiltAngle);
    }

    private void ApplyRotations(float tiltAngle, float pitchAngle)
    {
        // 应用主体旋转
        Quaternion targetRotation = Quaternion.Euler(0f, currentYaw, 0f);
        rb.MoveRotation(targetRotation);

        // 应用视角旋转
        if (trackingSpace != null)
        {
            Vector3 currentEuler = trackingSpace.eulerAngles;
            trackingSpace.eulerAngles = new Vector3(pitchAngle, currentEuler.y, tiltAngle);
        }
    }
    #endregion

    #region 高度控制
    private void DetectDive()
    {
        float averageHeight = trackingSpace.position.y - ((leftController.position.y + rightController.position.y) * 0.5f);

        if (averageHeight >= 0.05f)
        {
            verticalSpeed = defaultVerticalSpeed - averageHeight * gravityFactor;
            Debug.Log($"DetectDive: averageHeight={averageHeight}, defaultVerticalSpeed={defaultVerticalSpeed}, gravityFactor={gravityFactor}, verticalSpeed={verticalSpeed}");
        }
        else
        {
            float speedMultiplier = Mathf.Lerp(1f, 0.5f, Mathf.Abs(averageHeight) / 0.5f);
            verticalSpeed = defaultVerticalSpeed * speedMultiplier - averageHeight * gravityFactor;
        }
    }
    private void LimitPlayerHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            if (hit.distance > maxHeightFromGround)
            {
                Vector3 currentVelocity = rb.velocity;
                currentVelocity.y = Mathf.Min(currentVelocity.y, -1f);
                rb.velocity = currentVelocity;
            }
        }
    }

    private void CheckAndLiftIfGroundBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 20f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                StartCoroutine(GraduallyAddAndReduceUpwardVelocity(30f, 2f));
            }
        }
    }
    #endregion

    #region 碰撞处理
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
        // Debug.Log("Detected object: " + other.name);
    }

    private void HandleCollision(Collider other)
    {
        Vector3 contactPoint = other.ClosestPoint(transform.position);

        switch (other.tag)
        {
            case "Wall":
                HandleWallCollision(contactPoint);
                Debug.Log("111");
                break;
            case "pillar":
                HandlePillarCollision(other);
                break;
            case "DeadEnd":
                HandleDeadEnd();
                break;
            case "SpeedUp":
                HandleSpeedUpZone(true);
                break;
        }

        if (other.name == "RushToDeathArea")
        {
            HandleRushToDeathArea();
        }
    }

    private void HandleWallCollision(Vector3 contactPoint)
    {
        Vector3 wallNormal = contactPoint - transform.position;
        StartCoroutine(SmoothRotateParallelToWall(wallNormal, 1f));
        PlaySound(air_leaking);
    }

    private void HandlePillarCollision(Collider pillar)
    {
        // 计算柱子相对于玩家的位置
        Vector3 directionToPillar = pillar.transform.position - transform.position;
        // 获取横向方向（忽略Y轴）
        directionToPillar.y = 0;
        directionToPillar.Normalize();

        // 根据柱子位置决定推力方向
        pillarBoostDirection = directionToPillar;
        pillarBoostTimer = pillarBoostDuration;

        PlaySound(air_leaking);
    }

    private void HandleDeadEnd()
    {
        if (playableDirector != null && fadeScreen != null)
        {
            playableDirector.Play();
            fadeScreen.Fade(0f, 1f, 0.001f);
            RenderSettings.skybox.SetFloat("_Exposure", 0f);
        }
    }

    private void HandleRushToDeathArea()
    {
        isForcedToSkull = true;
        Vector3 directionToSkull = (skullTransform.position - transform.position).normalized;
        StartCoroutine(SmoothRotateToDirection(directionToSkull, 2f));
        PlaySound(dead);
        SpeedLine.SetActive(false);
        AudioManager.SetActive(false);
    }

    private void HandleSpeedUpZone(bool enter)
    {
        Debug.Log("enter: " + enter);
        isInSpeedUpZone = enter;
        if (enter)
        {
            // glideSpeed = originalGlideSpeed * speedUpMultiplier;
            defaultVerticalSpeed *= -speedUpMultiplier;  // 增加垂直速度
            gravityFactor = -55f;  // 增加重力因子
            Debug.Log("gravityFactor: " + gravityFactor + ", defaultVerticalSpeed: " + defaultVerticalSpeed);
        }
        else
        {
            glideSpeed = originalGlideSpeed;
            defaultVerticalSpeed /= -speedUpMultiplier;  // 恢复原始垂直速度
            gravityFactor = 0f;  // 恢复原始重力因子
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    #endregion

    #region 协程
    private IEnumerator GraduallyAddAndReduceUpwardVelocity(float maxUpwardSpeed, float duration)
    {
        isAddingUpwardVelocity = true;
        float halfDuration = duration * 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime < halfDuration
                ? elapsedTime / halfDuration
                : (elapsedTime - halfDuration) / halfDuration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float upwardVelocity = elapsedTime < halfDuration
                ? Mathf.Lerp(1f, maxUpwardSpeed, smoothT)
                : Mathf.Lerp(maxUpwardSpeed, 1f, smoothT);

            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = upwardVelocity;
            rb.velocity = currentVelocity;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAddingUpwardVelocity = false;
    }

    private IEnumerator SmoothRotateToDirection(Vector3 targetDirection, float duration)
    {
        isRotatingAway = true;
        Quaternion initialRotation = rb.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            rb.MoveRotation(Quaternion.Slerp(initialRotation, targetRotation, smoothT));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MoveRotation(targetRotation);
        yaw = targetRotation.eulerAngles.y;
        currentYaw = yaw;
        yawVelocity = 0f;
        isRotatingAway = false;
    }

    private IEnumerator SmoothRotateParallelToWall(Vector3 wallNormal, float duration)
    {
        isRotatingAway = true;
        Quaternion initialRotation = rb.rotation;
        Vector3 parallelDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        if (Vector3.Dot(parallelDirection, transform.forward) < 0)
        {
            parallelDirection = -parallelDirection;
        }

        Quaternion targetRotation = Quaternion.LookRotation(parallelDirection);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            rb.MoveRotation(Quaternion.Slerp(initialRotation, targetRotation, smoothT));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MoveRotation(targetRotation);
        yaw = targetRotation.eulerAngles.y;
        currentYaw = yaw;
        yawVelocity = 0f;
        isRotatingAway = false;
    }
    #endregion

    #region 事件处理
    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            // PlayerStateTran.Instance.Level1ToStage2();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SpeedUp"))
        {
            HandleSpeedUpZone(false);
        }
    }
    #endregion

    private void UpdatePillarBoost()
    {
        if (pillarBoostTimer > 0f)
        {
            pillarBoostTimer -= Time.deltaTime;
            // 应用横向推力
            rb.AddForce(pillarBoostDirection * pillarSideBoostForce * Time.deltaTime, ForceMode.Force);
        }
    }
}