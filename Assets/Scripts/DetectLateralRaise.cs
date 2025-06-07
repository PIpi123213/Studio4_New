using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectLateralRaise : MonoBehaviour
{
    public static DetectLateralRaise Instance { get; private set; } // 单例实例

    [SerializeField] private Transform leftHand; // 左手控制器
    [SerializeField] private Transform rightHand; // 右手控制器
    [SerializeField] private GameObject progressObject; // 带有shader的进度显示物体
    [SerializeField] private float requiredHeight = 1.2f; // 侧平举所需的最低高度
    [SerializeField] private float requiredDuration = 3f; // 需要保持的时间
    [SerializeField] private WingSuitMoveController wingsuitController; // 添加翼装控制器引用
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private PlayerStateTran playerStateTran; // 添加玩家状态引用

    private float currentDuration = 0f;
    private bool isLateralRaise = false;
    private bool hasActivatedWingsuit = false; // 添加标志位，确保只激活一次
    private Slider slider; // 用于存储shader材质

    // 公共属性，供其他脚本访问
    public bool IsLateralRaising => isLateralRaise;
    public float CurrentProgress => currentDuration / requiredDuration;
    public bool HasCompletedLateralRaise => hasActivatedWingsuit;

    void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 公共方法，供其他脚本调用
    public void ResetProgress()
    {
        currentDuration = 0f;
        hasActivatedWingsuit = false;
        if (slider != null)
        {
            slider.value = 0f;
        }
        if (progressObject != null)
        {
            progressObject.SetActive(false);
        }
    }

    public void SetRequiredDuration(float duration)
    {
        requiredDuration = duration;
    }

    public void SetRequiredHeight(float height)
    {
        requiredHeight = height;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 获取进度显示物体的Slider组件
        if (progressObject != null)
        {
            slider = progressObject.GetComponent<Slider>();
            if (slider != null)
            {
                // 初始化进度为0
                slider.value = 0f;
            }
            else
            {
                Debug.LogError("进度显示物体没有Slider组件！");
            }
            // 初始时隐藏进度物体
            progressObject.SetActive(false);
        }
        else
        {
            Debug.LogError("进度显示物体未设置！请在Inspector中设置Progress Object引用。");
        }

        // 确保开始时翼装控制器是禁用的
        if (wingsuitController != null)
        {
            //wingsuitController.enabled = false;
        }

        // 检查tutorialCanvas是否正确设置
        if (tutorialCanvas == null)
        {
            Debug.LogError("Tutorial Canvas 未设置！请在Inspector中设置Tutorial Canvas引用。");
        }

        // 确保物体有Collider组件
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("DetectLateralRaise需要添加一个Collider组件，并设置为Is Trigger!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检查PlayerStateTran的stage是否为1
        if (PlayerStateTran.Instance.Stage != 1) return;


        if (leftHand == null || rightHand == null) return;

        // 检查双手是否达到侧平举高度
        bool leftHandRaised = leftHand.localPosition.y >= requiredHeight;
        bool rightHandRaised = rightHand.localPosition.y >= requiredHeight;

        // 检查双手是否在身体两侧
        bool handsAtSides = true;
        isLateralRaise = leftHandRaised && rightHandRaised && handsAtSides;

        // 检测侧平举状态变化
        if (isLateralRaise && !hasActivatedWingsuit)
        {
            // 开始侧平举时显示进度物体
            if (progressObject != null)
            {
                progressObject.SetActive(true);
            }
        }
        else if (!isLateralRaise && hasActivatedWingsuit)
        {
            // 结束侧平举时隐藏进度物体
            if (progressObject != null)
            {
                progressObject.SetActive(false);
            }
        }

        hasActivatedWingsuit = isLateralRaise;

        if (isLateralRaise)
        {
            currentDuration += Time.deltaTime;
            if (currentDuration > requiredDuration)
            {
                currentDuration = requiredDuration;

                // 当达到要求时间且还未激活翼装时，激活翼装控制器
                if (!hasActivatedWingsuit && wingsuitController != null)
                {
                    wingsuitController.enabled = true;
                    hasActivatedWingsuit = true;

                    // 确保tutorialCanvas存在后再禁用
                    if (tutorialCanvas != null)
                    {
                        tutorialCanvas.SetActive(false);
                        Debug.Log("Tutorial Canvas 已禁用");
                    }
                    else
                    {
                        Debug.LogError("尝试禁用Tutorial Canvas时发现引用为空！");
                    }
                }
            }
        }
        else
        {
            currentDuration = 0f;
        }

        // 更新进度显示
        if (slider != null)
        {
            float progress = currentDuration / requiredDuration;
            slider.value = progress;
            if (slider.value == 1)
            {
                wingsuitController.enabled = true;
                tutorialCanvas.SetActive(false);
                playerStateTran.isStart = true;

            }
        }
    }
}