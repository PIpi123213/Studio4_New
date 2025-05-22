using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideFeedback : MonoBehaviour
{
    public GameObject decideButton;
    public GameObject giveUpFeedback;
    public GameObject acceptFeedback;
    
    private void Start()
    {
        if (giveUpFeedback != null)
        {
            giveUpFeedback.SetActive(false);
        }
        
        if (acceptFeedback != null)
        {
            acceptFeedback.SetActive(false);
        }
    }
    
    public void ShowGiveUpFeedback()
    {
        if (decideButton != null)
        {
            decideButton.SetActive(false);
        }
        if (giveUpFeedback != null)
        {
            giveUpFeedback.SetActive(true);
        }
        AudioManager.instance.PlayAudio("win");
    }
    
    public void ShowAcceptFeedback()
    {
        if (decideButton != null)
        {
            decideButton.SetActive(false);
        }
        if (acceptFeedback != null)
        {
            acceptFeedback.SetActive(true);
        }
        AudioManager.instance.PlayAudio("win");
    }
}
