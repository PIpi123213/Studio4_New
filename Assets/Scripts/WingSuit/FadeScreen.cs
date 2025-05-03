using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public  float    FadeDuration = 2f; // Fade duration in seconds
    private Renderer renderer;
    [SerializeField] bool fadeOnStart = true;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (fadeOnStart)
        {
            FadeIn(FadeDuration);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void FadeIn(float duration)
    {
        StartCoroutine(FadeRoutine(1f, 0f, duration));
    }
    public void FadeOut(float duration)
    {
        StartCoroutine(FadeRoutine(0f, 1f, duration));
    }
    public void Fade(float alphaIn, float alphaOut, float duration)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut, duration));
    }

    private IEnumerator FadeRoutine(float alphaIn, float alphaOut, float duration)
    {
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("WhiteFadeIn does not have a Renderer or Material.");
            yield break;
        }

        Material material     = renderer.material;
        Color    initialColor = material.color;
        Color    targetColor  = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Alpha = 0 (完全透明)

        float    elapsedTime  = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            targetColor.a           = Mathf.Lerp(alphaIn, alphaOut, t);
            renderer.material.color = targetColor;
            Debug.Log("透明度："+renderer.material.color.a);
            elapsedTime    += Time.deltaTime;


            yield return null;
        }
        renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alphaOut); // 确保最终颜色为目标颜色
    }
}