using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRockFalling : MonoBehaviour
{
    public AudioSource[] audioSources;
    public float minDelay = 0.1f;
    public float maxDelay = 0.5f;

    void Start()
    {
        foreach (var source in audioSources)
        {
            StartCoroutine(PlayWithLoopDelay(source));
        }
    }

    IEnumerator PlayWithLoopDelay(AudioSource source)
    {
        while (true) // 无限循环
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds( delay);
            source.Play();

            // 等待音效播放完毕 + 随机延迟
            yield return new WaitForSeconds(source.clip.length);
        }
    }

}
