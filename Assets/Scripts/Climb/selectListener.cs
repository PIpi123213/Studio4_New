using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class selectListener : MonoBehaviour
{
    // Start is called before the first frame update
    public CustomClimbInteractable climbInteractable;
    public GameObject hand_navi;
    void Start()
    {
        climbInteractable.selectEntered.AddListener(OnSelectEnter);
    }

    // Update is called once per frame
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
       hand_navi.SetActive(false);
      
    }

 
}
