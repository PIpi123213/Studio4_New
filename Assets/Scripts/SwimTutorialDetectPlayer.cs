using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimTutorialDetectPlayer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private float fadeSpeed = 1f; // 渐变速度
    private bool isFading = false;
    private bool hasPlayed = false; // 确保音频只播放一次

    [SerializeField] private AudioClip swimTutorialAudio;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("未找到SpriteRenderer组件！");
        }
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayAudioAfterDelay());
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
        audioSource.clip = swimTutorialAudio;
        audioSource.Play();

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
        if (!audioSource.isPlaying){
         gameObject.SetActive(false);
        }
    }
    private IEnumerator PlayAudioAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (audioSource != null && !hasPlayed)
        {
        audioSource.Play();
        hasPlayed = true;
    }
}
}