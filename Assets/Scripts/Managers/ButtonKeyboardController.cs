using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonKeyboardController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Button> buttons;
    private int currentIndex = 0;

    void Update()
    {
        // 上下方向键选择
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % buttons.Count;
            buttons[currentIndex].Select();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
            buttons[currentIndex].Select();
        }

        // 回车键确认
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            buttons[currentIndex].onClick.Invoke();
        }
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
