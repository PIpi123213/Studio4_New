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
            float delay = Random.Range(minDelay, maxDelay);
            source.PlayDelayed(delay);
        }
    }


}
