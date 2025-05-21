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
    #endregion

    #region 移动参数
    [Header("移动参数")]
    [SerializeField] private float glideSpeed = 1000f;
    [SerializeField] private float defaultVerticalSpeed;
    [SerializeField] private float gravityFactor = 10f;
    [SerializeField] private float maxTiltAngle = 30f;
    [SerializeField] private float rotationSensitivity = 1.5f;
    [SerializeField] private float maxHeightFromGround = 70f;
    #endregion

    #region 私有变量
    private float verticalSpeed;
    private float yaw = 0f;
    private float currentYaw;
    private float yawVelocity = 0f;
    private bool isRotatingAway = false;
    private bool isForcedToSkull = false;
    private bool isAddingUpwardVelocity = false;
    #endregion

    #region Unity生命周期
    private void Start()
    {
        InitializeComponents();
        SubscribeToEvents();
    }

    private void Update()
    {
        if (!PlayerStateTran.Instance.isStart) return;

        HandleMovement();
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
        Vector3 glideVelocity = transform.forward * glideSpeed;
        glideVelocity.y = verticalSpeed;
        rb.velocity = glideVelocity;
    }

    private void ApplyRotation()
    {
        if (isRotatingAway) return;

        // 计算左右手高度差和平均高度
        float heightDifference = leftController.position.y - rightController.position.y;
        float averageHeight = (leftController.position.y + rightController.position.y) * 0.5f;

        // 更新偏航角
        UpdateYaw(heightDifference);

        // 计算倾斜和俯仰角度
        float tiltAngle = CalculateTiltAngle(heightDifference);
        float pitchAngle = CalculatePitchAngle(averageHeight);

        // 应用旋转
        ApplyRotations(tiltAngle, pitchAngle);
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
        float pitchAngle = -(averageHeight - trackingSpace.position.y) * maxTiltAngle;
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
    }

    private void HandleCollision(Collider other)
    {
        Vector3 contactPoint = other.ClosestPoint(transform.position);

        switch (other.tag)
        {
            case "Wall":
                HandleWallCollision(contactPoint);
                break;
            case "pillar":
                HandlePillarCollision(other);
                break;
            case "DeadEnd":
                HandleDeadEnd();
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
        Vector3 directionToObject = pillar.transform.position - transform.position;
        float dotProduct = Vector3.Dot(transform.right, directionToObject);

        if (Vector3.Dot(transform.forward, directionToObject) < 0)
        {
            Vector3 targetDirection = dotProduct > 0
                ? (transform.forward - transform.right).normalized
                : (transform.forward + transform.right).normalized;

            StartCoroutine(SmoothRotateToDirection(targetDirection, 1f));
        }

        PlaySound(air_leaking);
    }

    private void HandleDeadEnd()
    {
        if (playableDirector != null && fadeScreen != null)
        {
            playableDirector.Play();
            fadeScreen.Fade(0f, 1f, 0.001f);
        }
    }

    private void HandleRushToDeathArea()
    {
        isForcedToSkull = true;
        Vector3 directionToSkull = (skullTransform.position - transform.position).normalized;
        StartCoroutine(SmoothRotateToDirection(directionToSkull, 2f));
        PlaySound(dead);
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
            PlayerStateTran.Instance.Level1ToStage2();
        }
    }
    #endregion
}