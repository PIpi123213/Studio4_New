using AmazingAssets.DynamicRadialMasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Trigger3 : MonoBehaviour
{
    // Start is called before the first frame update
    private XRGrabInteractable grabInteractable;
    [SerializeField] private DRMGameObject drmGameObject;
    [SerializeField] private OVRPassthroughLayer ptLayer;
    bool hasTriggered = false;
    private bool radiusFinished = false;
    private bool opacityFinished = false;
    public Camera playercamera;
    //public GameObject Arinteraction;
    [SerializeField] float skyboxFadeDuration = 10f;

    public GameObject Level2;
    public GameObject BCI;
    public GameObject Arinteraction;
    public GameObject Locker;


    private void Awake()
    {
       
    }
    void Start()
    {
     
      
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("catch it��");       
        if (!hasTriggered)
        {



        }
    }
    public void Level2ChangeToStage3()
    {
        SceneTransitionManager.Instance.StartPreloading("Ocean");
        Level2.SetActive(false);
        BCI.SetActive(true);
        Arinteraction.SetActive(false);
        Locker.SetActive(false);

    }

    public void StartAnimate()
    {
        StartCoroutine(RunBothAnimations());
    }
    private IEnumerator RunBothAnimations()
    {
        // ͬʱ������������
        hasTriggered = true;
        Coroutine radiusRoutine = StartCoroutine(AnimateRadius());
       // Coroutine opacityRoutine = StartCoroutine(AnimateOpacity());
        // �ȴ�������ɣ���ʱ��ȡ���ֵ��
        yield return radiusRoutine;
       // yield return opacityRoutine;
        Debug.Log("All animations completed!");
    }

    [SerializeField] float RadiusDuration = 5f; 
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 100f;

    private IEnumerator AnimateRadius()
    {

        float elapsedTime = 0f;

        while (elapsedTime < RadiusDuration)
        {
            // ʹ�÷����Բ�ֵ����
            float t = Mathf.Pow(elapsedTime / RadiusDuration, 2); // ��������
            drmGameObject.radius = Mathf.Lerp(startRadius, endRadius, t);

            if (drmGameObject.radius > 250)
            {
                //Arinteraction.SetActive(false);
                playercamera.clearFlags = CameraClearFlags.Skybox;
                float extraSpeedFactor = 5f; // �ɸ�����Ҫ�������ٱ���
                float extraT = Mathf.Pow(elapsedTime / RadiusDuration, 2) * extraSpeedFactor;
                drmGameObject.radius += Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;
             

            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hasTriggered = true;
        radiusFinished = true;
        drmGameObject.radius = endRadius;
    }
   



    [SerializeField] float opacityDuration = 5f;
    [SerializeField] float opacityDuration_out = 2f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;
    private IEnumerator AnimateOpacity()
    {
        resetSkybox();
        float elapsedTime = 0f;



        while (elapsedTime < opacityDuration)
        {
            ptLayer.textureOpacity = Mathf.Lerp(startOpacity, endOpacity, elapsedTime / opacityDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        opacityFinished = true;
        hasTriggered = true;
        ptLayer.textureOpacity = endOpacity;

    }
    private IEnumerator AnimateOpacity_out()
    {
        resetSkybox();
        float elapsedTime = 0f;



        while (elapsedTime < opacityDuration_out)
        {
            ptLayer.textureOpacity = Mathf.Lerp(endOpacity, startOpacity, elapsedTime / opacityDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        opacityFinished = true;
        hasTriggered = true;
        ptLayer.textureOpacity = startOpacity;

    }
    public IEnumerator AnimateSkybox_out()
    {

        Coroutine skyboxRoutine = StartCoroutine(AnimateSkyboxExposure(1f, 0f, 5f));
        yield return skyboxRoutine;
    }

    public IEnumerator AnimateSkyboxExposure(float startExposure, float endExposure, float duration)
    {
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float exposure = Mathf.Lerp(startExposure, endExposure, elapsedTime / duration);
                RenderSettings.skybox.SetFloat("_Exposure", exposure);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 确保最终曝光度为目标值
            RenderSettings.skybox.SetFloat("_Exposure", endExposure);
        }
    }

    public void resetSkybox()
    {
        playercamera.clearFlags = CameraClearFlags.SolidColor;

    }
  
    void OnDestroy()
    {
        // ȡ�������¼�
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }

   
}
