using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimAudioManager : MonoBehaviour
{
    // Start is called before the first frame update
     public AudioClip newBGM;           // 拖入bgmB
    private AudioSource audioSource;

    void Start()
    {
        // 可以找到主摄像机上的AudioSource，也可以在这里设置一个指定的 AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // 玩家进入触发区域
        if (other.CompareTag("Player")) // 你的玩家对象应该设置为 "Player" 标签
        {
            if (audioSource.clip != newBGM)
            {
                audioSource.clip = newBGM;
                audioSource.Play();
            }
        }
    }
    IEnumerator Crossfade(AudioClip newClip, float fadeTime = 1f)
{
    float startVolume = audioSource.volume;

    // 淡出
    while (audioSource.volume > 0)
    {
        audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
        yield return null;
    }

    audioSource.clip = newClip;
    audioSource.Play();

    // 淡入
    while (audioSource.volume < startVolume)
    {
        audioSource.volume += startVolume * Time.deltaTime / fadeTime;
        yield return null;
    }

    audioSource.volume = startVolume;
}

}
