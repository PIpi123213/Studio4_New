using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class ButtonKeyboardController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Button> buttons;
    private int currentIndex = 0;

    public InputActionProperty buttonA;
    public InputActionProperty buttonB;
    public InputActionProperty rightbuttonA;
    public InputActionProperty rightbuttonB;
    // 上一帧状态
    private bool lastAPressed = false;
    private bool lastBPressed = false;
    private bool lastRightAPressed = false;
    private bool lastRightBPressed = false;

    void Update()
    {
        // 当前帧是否按下
        bool aPressed = buttonA.action.IsPressed();
        bool bPressed = buttonB.action.IsPressed();
        bool rightaPressed = rightbuttonA.action.IsPressed();
        bool rightbPressed = rightbuttonB.action.IsPressed();

        // 检测“刚按下”这帧（类似 GetKeyDown）
        bool aJustPressed = aPressed && !lastAPressed;
        bool bJustPressed = bPressed && !lastBPressed;
        bool rightaJustPressed = rightaPressed && !lastRightAPressed;
        bool rightbJustPressed = rightbPressed && !lastRightBPressed;

        // ↓：向下切换
        if (Input.GetKeyDown(KeyCode.DownArrow) || rightbJustPressed)
        {
            currentIndex = (currentIndex + 1) % buttons.Count;
            buttons[currentIndex].Select();
        }

        // ↑：向上切换
        else if (Input.GetKeyDown(KeyCode.UpArrow) || rightaJustPressed)
        {
            currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
            buttons[currentIndex].Select();
        }

        // 回车/空格/A/B：确认
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || aJustPressed || bJustPressed)
        {
            buttons[currentIndex].onClick.Invoke();
        }

        // 更新上一帧状态
        lastAPressed = aPressed;
        lastBPressed = bPressed;
        lastRightAPressed = rightaPressed;
        lastRightBPressed = rightbPressed;
    }

    void Start()
    {
        // 初始选择第一个按钮
        if (buttons.Count > 0)
        {
            buttons[0].Select();
        }
    }
}
