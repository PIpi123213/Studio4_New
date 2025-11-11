using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Enddetect : MonoBehaviour
{
    private bool isFinished = false;
    private bool isStart = false;
    [SerializeField] float fadeDuration = 3f;   // 渐变总时长
    public FadeScreen fadeScreen;              // 引用 FadeScreen 脚本

    public Renderer renderer;
    private Coroutine fadeCoroutine;
    public PlayableDirector Endtimeline;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!isStart)
        {
            Debug.Log("游戏开始");
            isStart = true;
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // 从当前值 → 1
            float currentAlpha = renderer.material.color.a;
            fadeCoroutine = StartCoroutine(FadeRoutine(currentAlpha, 1f, fadeDuration));
            // 玩家进入 → 渐渐变透明（alpha: 1 → 0）

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")&&!isFinished)
        {
            // 玩家离开 → 渐渐恢复（alpha: 当前值 → 1）
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            isStart = false;
            // 从当前值 → 0
            float currentAlpha = renderer.material.color.a;
            fadeCoroutine = StartCoroutine(FadeRoutine(currentAlpha, 0f, 0.2f));
        }
    }


    private IEnumerator FadeRoutine(float alphaIn, float alphaOut, float duration)
    {
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("WhiteFadeIn does not have a Renderer or Material.");
            yield break;
        }

        Material material = renderer.material;
        Color initialColor = material.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Alpha = 0 (完全透明)

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            targetColor.a = Mathf.Lerp(alphaIn, alphaOut, t);
            renderer.material.color = targetColor;
            //Debug.Log("透明度："+renderer.material.color.a);
            elapsedTime += Time.deltaTime;


            yield return null;
        }
        renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alphaOut);

        isFinished = true;
        Endtimeline.Play();// 确保最终颜色为目标颜色
    }
}
