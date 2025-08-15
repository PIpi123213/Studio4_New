using AmazingAssets.DynamicRadialMasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TriggerCourtroom : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    //private CustomClimbInteractable grabInteractable;
    // [SerializeField] private GameObject         DissolveEffectTool;
    [SerializeField] private DRMGameObject drmGameObject;
    [SerializeField] private OVRPassthroughLayer ptLayer;
    bool hasTriggered = false;
    private bool radiusFinished = false;
    private bool opacityFinished = false;
    public Camera playercamera;


    //public AudioSource BGM;

    private void Awake()
    {
        
        //ResetPostprocess();
    }
    void Start()
    {
      
        drmGameObject.radius = 80f;
        RenderSettings.skybox.SetFloat("_Exposure", 0);

        ptLayer.textureOpacity = 0f;

    }
  


  
    public void courtroomdissapear()
    {

        StartCoroutine(AnimateRadius_out_first());




    }




    [SerializeField] float RadiusDuration = 5f;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 80f;
    private IEnumerator AnimateRadius_out_first()
    {
        float elapsedTime = 0f;

        while (elapsedTime < RadiusDuration)
        {
            // ʹ�÷����Բ�ֵ����
            float t = 1 - Mathf.Pow(1 - (elapsedTime / RadiusDuration), 2); // ��������
            drmGameObject.radius = Mathf.Lerp(endRadius, startRadius, t);

            //playercamera.clearFlags = CameraClearFlags.Skybox;
            float extraSpeedFactor = 5f; // �ɸ�����Ҫ�������ٱ���
            float extraT = 1 - Mathf.Pow(1 - (elapsedTime / RadiusDuration), 2) * extraSpeedFactor;

            // ���Ӷ���İ뾶����
            drmGameObject.radius -= Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;

            //Debug.Log("vfx");


            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ʾ�����룺����Passthrough��ָ�����

        drmGameObject.radius = startRadius;
    }



    [SerializeField] float opacityDuration = 5f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;
    private IEnumerator AnimateOpacity()
    {

        float elapsedTime = 0f;



        while (elapsedTime < opacityDuration)
        {
            ptLayer.textureOpacity = Mathf.Lerp(endOpacity, startOpacity,  elapsedTime / opacityDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        opacityFinished = true;
        hasTriggered = true;
        ptLayer.textureOpacity = startOpacity;

    }



}
