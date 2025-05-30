using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 添加UI命名空间

public class DetectLateralRaise : MonoBehaviour
{
    [SerializeField] private Transform leftHand; // 左手控制器
    [SerializeField] private Transform rightHand; // 右手控制器
    [SerializeField] private Slider progressBar; // 进度条
    [SerializeField] private float requiredHeight = 1.5f; // 侧平举所需的最低高度
    [SerializeField] private float requiredDuration = 3f; // 需要保持的时间
    [SerializeField] private WingSuitMoveController wingsuitController; // 添加翼装控制器引用
    [SerializeField] private GameObject tutorialCanvas; // 添加翼装对象引用
    [SerializeField] private PlayerStateTran playerStateTran; // 添加玩家状态引用

    private float currentDuration = 0f;
    private bool isLateralRaise = false;
    private bool hasActivatedWingsuit = false; // 添加标志位，确保只激活一次

    // Start is called before the first frame update
    void Start()
    {
        if (progressBar != null)
        {
            progressBar.maxValue = requiredDuration;
            progressBar.value = 0f;
        }

        // 确保开始时翼装控制器是禁用的
        if (wingsuitController != null)
        {
            wingsuitController.enabled = false;
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
        bool leftHandRaised = leftHand.position.y >= requiredHeight;
        bool rightHandRaised = rightHand.position.y >= requiredHeight;

        // 检查双手是否在身体两侧
        bool handsAtSides = Mathf.Abs(leftHand.position.x) > 0.3f && Mathf.Abs(rightHand.position.x) > 0.3f;

        isLateralRaise = leftHandRaised && rightHandRaised && handsAtSides;

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

        // 更新进度条
        if (progressBar != null)
        {
            progressBar.value = currentDuration;
        }
    }
}