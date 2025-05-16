using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class CharacterClimb : MonoBehaviour
{
    // Start is called before the first frame update
    public CustomClimbProvider climbProvider;

    public ZipLine zipline;
    public static bool isClimbing = false;
    public static bool isStart = false;
    void Start()
    {
        if (climbProvider == null)
        {
            Debug.LogError("ClimbProvider 未找到！");
        }
    
       
    }

    // Update is called once per frame
    void Update()
    {
        if (climbProvider == null )
            return;

        // 检测攀爬状态
        isClimbing = climbProvider.locomotionPhase == LocomotionPhase.Moving ||
                          climbProvider.locomotionPhase == LocomotionPhase.Started || ZipLine.isSliding;



    }
}
