using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingsuitTurnRightTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isTriggered = false;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            audioSource.Play();
            isTriggered = true;

        }
    }
}