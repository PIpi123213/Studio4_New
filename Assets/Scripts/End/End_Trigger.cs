using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class End_Trigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private OVRPassthroughLayer Windows_ptLayer;
    [SerializeField] private OVRPassthroughLayer Global_ptLayer;
    private bool isEnd = false;
    
    public GameObject Logo;
    public float fadeDuration = 2.0f;
    public void Awake()
    {
        Windows_ptLayer.textureOpacity = 0f;
        Global_ptLayer.textureOpacity = 1f;
    }

    void Start()
    {
        Logo.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!isEnd)
        {
            Debug.Log("��Ϸ����");

            StartCoroutine(FadeBackgroundValue());
            isEnd = true;
        }

    }
    IEnumerator FadeBackgroundValue()
    {
        // 获取当前背景色的HSV值
        Color initialColor = Camera.main.backgroundColor;
        Color.RGBToHSV(initialColor, out float h, out float s, out float initialV);

        float targetV = 0f; // 目标明度（纯黑）
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // 计算当前明度（从初始V值降到0）
            float currentV = Mathf.Lerp(initialV, targetV, elapsedTime / fadeDuration);

            // 用HSV转换回RGB颜色，并设置背景
            Camera.main.backgroundColor = Color.HSVToRGB(h, s, currentV);

            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保最终颜色精确设置为黑色（V=0）
        Camera.main.backgroundColor = Color.HSVToRGB(h, s, targetV);
        Debug.Log("背景明度渐变完成！");
    }

    [SerializeField] float opacityDuration = 100f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;
    
    public void WindosShowup()
    {
        StartCoroutine(AnimateOpacity(Windows_ptLayer));
    }

    private IEnumerator AnimateOpacity(OVRPassthroughLayer ptLayer)
    {
        float elapsedTime = 0f;

        while (elapsedTime < opacityDuration)
        {
            ptLayer.textureOpacity = Mathf.Lerp(startOpacity, endOpacity, elapsedTime / opacityDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        ptLayer.textureOpacity = endOpacity;

        yield return new WaitForSeconds(2f);
        Logo.SetActive(true);
    }
}
