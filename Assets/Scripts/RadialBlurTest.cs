using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OccaSoftware.RadialBlur.Demo;
using UnityEngine.XR.Interaction.Toolkit;

public class RadialBlurTest : MonoBehaviour
{
    [SerializeField] private SetRadialBlurExamples radialBlur;

    // Start is called before the first frame update
    void Start()
    {
        radialBlur.SetIntensity(0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("OnSelectEnter");
        radialBlur.SetIntensity(0.5f);
    }
}
