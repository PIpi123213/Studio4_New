using AmazingAssets.DynamicRadialMasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Playables;


public class Trigger0 : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private XRGrabInteractable grabInteractable;
    // [SerializeField] private GameObject         DissolveEffectTool;
    [SerializeField] private DRMGameObject drmGameObject;
    [SerializeField] private OVRPassthroughLayer ptLayer;
    bool hasTriggered = false;
    private bool radiusFinished = false;
    private bool opacityFinished = false;
    public Camera playercamera;
    public PlayableDirector letterTimeline;

    public GameObject VFX;

    //public GameObject postprocess;

    private void Awake()
    {
        RenderSettings.skybox.SetFloat("_Exposure", 0f);
    }
    void Start()
    {
        StartCoroutine(findCamera());
        grabInteractable = GetComponent<XRGrabInteractable>();

        // ���� Select Enter �¼�
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        drmGameObject.radius = 0;
        //RenderSettings.skybox.SetFloat("_Exposure", 0);
        VFX.SetActive(false);


    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("catch it��");
        letterTimeline.Play();

        if (!hasTriggered)
        {
            //StartCoroutine(AnimateRadius());
            //StartCoroutine(AnimateOpacity());
            StartCoroutine(RunBothAnimations());
        }
    }
    [SerializeField] float skyboxFadeDuration = 10f;
    private IEnumerator RunBothAnimations()
    {
        // ͬʱ������������
        hasTriggered = true;

        Coroutine radiusRoutine = StartCoroutine(AnimateRadius());
        Coroutine opacityRoutine = StartCoroutine(AnimateOpacity());

        // �ȴ�������ɣ���ʱ��ȡ���ֵ��
        yield return radiusRoutine;
        yield return opacityRoutine;
        //��������


        //MoveManager.Instance.OnSceneIn();//��¼λ��
        // ��ɺ�ִ�г����л��������߼�
        Debug.Log("All animations completed!");



    }
    public void Endanimation()
    {
        StartCoroutine(RunBothAnimations_out());



    }
    public IEnumerator RunBothAnimations_out()
    {


        Coroutine radiusRoutine = StartCoroutine(AnimateRadius_out());
        //Coroutine opacityRoutine = StartCoroutine(AnimateOpacity_out());
        Coroutine SkyopacityRoutine = StartCoroutine(AnimateSkybox_out());

        yield return radiusRoutine;
        yield return SkyopacityRoutine;


        //MoveManager.Instance.OnSceneIn();




    }






    [SerializeField] float RadiusDuration = 25f;
    [SerializeField] float RadiusDuration_out = 5f;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 100f;

    private IEnumerator AnimateRadius()
    {
        yield return new WaitForSeconds(20f);
        float elapsedTime = 0f;

        float remainingMovementTime = 0f;

        while (elapsedTime < RadiusDuration)
        {
            // ʹ�÷����Բ�ֵ����
            float t = Mathf.Pow(elapsedTime / RadiusDuration, 2); // ��������
            drmGameObject.radius = Mathf.Lerp(startRadius, endRadius, t);
            if(drmGameObject.radius > 20)
            {
                VFX.SetActive(true);
            }
            if (drmGameObject.radius > 250)
            {
                playercamera.clearFlags = CameraClearFlags.Skybox;
                float extraSpeedFactor = 5f; // �ɸ�����Ҫ�������ٱ���
                float extraT = Mathf.Pow(elapsedTime / RadiusDuration, 2) * extraSpeedFactor;

                // ���Ӷ���İ뾶����
                drmGameObject.radius += Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;
                VFX.SetActive(true);
            }
            if (drmGameObject.radius > 450)
            {

                float extraSpeedFactor = 5f;
                float extraT = Mathf.Pow(elapsedTime / RadiusDuration, 2) * extraSpeedFactor;
                // 额外的半径增长
                drmGameObject.radius += Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;


            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ʾ�����룺����Passthrough��ָ�����


        hasTriggered = true;
        radiusFinished = true;
        drmGameObject.radius = endRadius;
    }
    public IEnumerator AnimateRadius_out()
    {

        float elapsedTime = 0f;

        float remainingMovementTime = 0f;

        while (elapsedTime < RadiusDuration_out)
        {
            // ʹ�÷����Բ�ֵ����
            float t = 1 - Mathf.Pow(1 - (elapsedTime / RadiusDuration_out), 2); // ��������
            drmGameObject.radius = Mathf.Lerp(endRadius, startRadius, t);
            if (drmGameObject.radius < 250)
            {
                //playercamera.clearFlags = CameraClearFlags.Skybox;
                float extraSpeedFactor = 5f; // �ɸ�����Ҫ�������ٱ���
                float extraT = 1 - Mathf.Pow(1 - (elapsedTime / RadiusDuration_out), 2) * extraSpeedFactor;

                // ���Ӷ���İ뾶����
                drmGameObject.radius -= Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;
                VFX.SetActive(false);
                //Debug.Log("vfx");
            }
            if (drmGameObject.radius < 450)
            {

                float extraSpeedFactor = 5f;
                float extraT = 1 - Mathf.Pow(1 - (elapsedTime / RadiusDuration_out), 2) * extraSpeedFactor;
                // 额外的半径增长
                drmGameObject.radius -= Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;

                // 计算剩余时间比例

            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ʾ�����룺����Passthrough��ָ�����

        // hasTriggered = true;
        //radiusFinished = true;
        drmGameObject.radius = startRadius;
    }



    [SerializeField] float opacityDuration = 100f;
    [SerializeField] float opacityDuration_out = 2f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;
    private IEnumerator AnimateOpacity()
    {
        yield return new WaitForSeconds(10f);
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
        if (ptLayer != null)
        {
            ptLayer.enabled = false;
            // Destroy(ptLayer);
        }
        playercamera.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox.SetFloat("_Exposure", 0f);

        //SetupPostprocess();
        /* Coroutine skyboxRoutine = StartCoroutine(AnimateSkyboxExposure(0f, 1f, skyboxFadeDuration));
         yield return skyboxRoutine;*/

    }
    private IEnumerator AnimateOpacity_out()
    {

        float elapsedTime = 0f;



        while (elapsedTime < opacityDuration_out)
        {
            ptLayer.textureOpacity = Mathf.Lerp(endOpacity, startOpacity, elapsedTime / opacityDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // opacityFinished = true;
        // hasTriggered = true;
        ptLayer.textureOpacity = startOpacity;
        if (ptLayer != null)
        {
            ptLayer.enabled = true;
            // Destroy(ptLayer);
        }
        //SetupPostprocess();


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
        RenderSettings.skybox.SetFloat("_Exposure", 1f);
        playercamera.clearFlags = CameraClearFlags.Skybox;

    }

    void OnDestroy()
    {
        // ȡ�������¼�
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }
    private IEnumerator findCamera()
    {
        yield return null;
        // 等待一帧
        GameObject camera1 = GameObject.FindWithTag("MainCamera");
        if (camera1 == null)
        {
            Debug.LogError("找不到 Tag 为 'PlayerCamera' 的 GameObject！");
            yield break; // 如果是协程，提前退出
        }
        playercamera = camera1.GetComponent<Camera>();
        if (playercamera == null)
        {
            Debug.LogError("找到的 GameObject 没有 Camera 组件！");
            yield break;
        }


    }

}