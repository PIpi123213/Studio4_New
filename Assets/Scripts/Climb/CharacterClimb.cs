using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class CharacterClimb : MonoBehaviour
{
    // Start is called before the first frame update
    public CustomClimbProvider climbProvider;
    public DynamicMoveProvider dynamicMoveProvider;
    public ZipLine zipline;
    public static bool isClimbing = false;
    public static bool isStart = false;
    void Start()
    {
        if (climbProvider == null)
        {
            Debug.LogError("ClimbProvider δ�ҵ���");
        }
        if (dynamicMoveProvider == null)
        {
            Debug.LogError("DynamicMoveProvider δ�ҵ���");
        }
        if(zipline == null)
        {
            Debug.LogError("zipline δ�ҵ���");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (climbProvider == null || dynamicMoveProvider == null)
            return;

        // �������״̬
        isClimbing = climbProvider.locomotionPhase == LocomotionPhase.Moving ||
                          climbProvider.locomotionPhase == LocomotionPhase.Started || ZipLine.isSliding;



        // ��������������ر�����������������
        //dynamicMoveProvider.useGravity = !isClimbing;
    }
}
