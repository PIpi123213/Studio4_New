using UnityEngine;
using System.Collections;
using TMPro;

public class AutoPasswordChecker : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText;

    public GameObject inputField;
    public GameObject decideButton;

    private string correctPassword = "1234";

    void Start()
    {
        // 监听输入变化
        passwordInput.onValueChanged.AddListener(OnPasswordChanged);
        passwordInput.characterLimit = 4; // 限制最大长度为4
        /*
        passwordInput.contentType = TMP_InputField.ContentType.Password; // 隐藏输入
    */
        if (decideButton != null)
        {
            decideButton.SetActive(false);
        }
    }

    void OnPasswordChanged(string input)
    {
        if (input.Length == 4)
        {
            CheckPassword(input);
        }
        else
        {
            feedbackText.text = ""; // 清空提示
        }
    }

    void CheckPassword(string input)
    {
        if (input == correctPassword)
        {
            decideButton.SetActive(true);
            inputField.SetActive(false);
            AudioManager.instance.PlayAudio("startUp");
        }
        else
        {
            feedbackText.text = "Wrong Password!";
            feedbackText.color = Color.red;
            StartCoroutine(ResetInputField());
        }
    }
    
    IEnumerator ResetInputField()
    {
        passwordInput.text = ""; // 清空输入框
        yield return new WaitForSeconds(2f);
        feedbackText.text = "";  // 清空提示文本
    }
}