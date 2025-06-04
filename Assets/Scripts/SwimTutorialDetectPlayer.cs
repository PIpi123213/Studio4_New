using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimTutorialDetectPlayer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeSpeed = 1f; // 渐变速度
    private bool isFading = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("未找到SpriteRenderer组件！");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的对象是否是玩家
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        if (spriteRenderer == null) yield break;

        isFading = true;
        Color color = spriteRenderer.color;

        while (color.a > 0)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = color;
            yield return null;
        }

        // 确保完全透明
        color.a = 0;
        spriteRenderer.color = color;

        // 可选：完全隐藏物体
        gameObject.SetActive(false);
    }
}