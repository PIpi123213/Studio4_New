using AmazingAssets.DynamicRadialMasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class Trigger2 : MonoBehaviour
{
    // Start is called before the first frame update
    private XRGrabInteractable grabInteractable;
    // [SerializeField] private GameObject         DissolveEffectTool;
    [SerializeField] private DRMGameObject drmGameObject;
    [SerializeField] private OVRPassthroughLayer ptLayer;
    bool hasTriggered = false;
    private bool radiusFinished = false;
    private bool opacityFinished = false;
    public Camera playercamera;
    public AttachAnchor attachAnchor;
    public GameObject lake;
    public Vector3 lakeTargetPosition = new Vector3(0, 0, 0);
    public Vector3 lakeStartPosition = new Vector3(0, 0, 0);
    public GameObject VFX;
    public GameObject Arinteraction;
    public GameObject FirstRope;
    //public GameObject postprocess;

    private void Awake()
    {
        lake.transform.position = lakeStartPosition;
        //ResetPostprocess();
    }
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        //StartCoroutine(findCamera());
        // ���� Select Enter �¼�
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        drmGameObject.radius = 0;
        RenderSettings.skybox.SetFloat("_Exposure", 0);
        lake.SetActive(false);
        VFX.SetActive(false);

       



    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("catch it��");
        // ���磺�ı������ɫ
        GetComponent<Renderer>().material.color = Color.red;
        if (!hasTriggered)
        {
            //StartCoroutine(AnimateRadius());
            //StartCoroutine(AnimateOpacity());
            StartCoroutine(RunBothAnimations());
        }
    }
    [SerializeField]float skyboxFadeDuration = 10f;
    private IEnumerator RunBothAnimations()
    {
        // ͬʱ������������
        hasTriggered = true;

        Coroutine radiusRoutine  = StartCoroutine(AnimateRadius());
        Coroutine opacityRoutine = StartCoroutine(AnimateOpacity());
        attachAnchor.Attach();
        // �ȴ�������ɣ���ʱ��ȡ���ֵ��
        yield return radiusRoutine;
        yield return opacityRoutine;
        //��������

        
        //MoveManager.Instance.OnSceneIn();//��¼λ��
        // ��ɺ�ִ�г����л��������߼�
        Debug.Log("All animations completed!");
        
        //gameObject.SetActive(false);


    }
    public void Endanimation()
    {
        StartCoroutine(RunBothAnimations_out());



    }
    public IEnumerator RunBothAnimations_out()
    {
       
        FirstRope.SetActive(false);
        Coroutine radiusRoutine = StartCoroutine(AnimateRadius_out());
        //Coroutine opacityRoutine = StartCoroutine(AnimateOpacity_out());
        Coroutine SkyopacityRoutine = StartCoroutine(AnimateSkybox_out());
      
        yield return radiusRoutine;
        yield return SkyopacityRoutine;
 

        //MoveManager.Instance.OnSceneIn();
     
      


    }






    [SerializeField] float RadiusDuration = 5f;
    [SerializeField] float RadiusDuration_out = 5f;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 100f;

    private IEnumerator AnimateRadius()
    {

        float elapsedTime = 0f;
        
        float remainingMovementTime = 0f;

        while (elapsedTime < RadiusDuration)
        {
            // ʹ�÷����Բ�ֵ����
            float t = Mathf.Pow(elapsedTime / RadiusDuration, 2); // ��������
            drmGameObject.radius = Mathf.Lerp(startRadius, endRadius, t);
            if (drmGameObject.radius > 250)
            {
                Arinteraction.SetActive(false);
                playercamera.clearFlags = CameraClearFlags.Skybox;
                float extraSpeedFactor = 5f; // �ɸ�����Ҫ�������ٱ���
                float extraT = Mathf.Pow(elapsedTime / RadiusDuration, 2) * extraSpeedFactor;
                
                // ���Ӷ���İ뾶����
                drmGameObject.radius += Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;
                VFX.SetActive(true);
            }
            if (drmGameObject.radius > 450)
            {
                if (!lake.activeSelf)
                {
                    lake.SetActive(true);
                    lakeStartPosition = lake.transform.position;
                    remainingMovementTime = RadiusDuration - elapsedTime;

                }
                float extraSpeedFactor = 5f;
                float extraT = Mathf.Pow(elapsedTime / RadiusDuration, 2) * extraSpeedFactor;
                // 额外的半径增长
                drmGameObject.radius += Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;

                // 计算剩余时间比例
                if (remainingMovementTime > 0)
                {
                    float moveProgress = 1f - (RadiusDuration - elapsedTime) / remainingMovementTime;
                    moveProgress = Mathf.Clamp01(moveProgress);

                    // 使用线性移动确保在剩余时间内完成
                    lake.transform.position = Vector3.Lerp(lakeStartPosition, lakeTargetPosition, moveProgress);


                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ʾ�����룺����Passthrough��ָ�����

        lake.transform.position = lakeTargetPosition;
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
                float extraT = 1-Mathf.Pow(1- (elapsedTime / RadiusDuration_out), 2) * extraSpeedFactor;

                // ���Ӷ���İ뾶����
                drmGameObject.radius -= Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;
                VFX.SetActive(false);
                //Debug.Log("vfx");
            }
            if (drmGameObject.radius < 450)
            {
              
                float extraSpeedFactor = 5f;
                float extraT = 1-Mathf.Pow(1- (elapsedTime / RadiusDuration_out), 2) * extraSpeedFactor;
                // 额外的半径增长
                drmGameObject.radius -= Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;

                // 计算剩余时间比例
                if (remainingMovementTime > 0)
                {
                    float moveProgress = 1f - (RadiusDuration_out - elapsedTime) / remainingMovementTime;
                    moveProgress = Mathf.Clamp01(moveProgress);

                    // 使用线性移动确保在剩余时间内完成
                    lake.transform.position = Vector3.Lerp(lakeTargetPosition, lakeStartPosition, moveProgress);


                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ʾ�����룺����Passthrough��ָ�����

        lake.SetActive(false);
        // hasTriggered = true;
        //radiusFinished = true;
        drmGameObject.radius = startRadius;
    }



    [SerializeField] float opacityDuration = 5f;
    [SerializeField] float opacityDuration_out = 2f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;
    private IEnumerator AnimateOpacity()
    {

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
        //SetupPostprocess();
        Coroutine skyboxRoutine = StartCoroutine(AnimateSkyboxExposure(0f, 1f, skyboxFadeDuration));
        yield return skyboxRoutine;

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
   /* public void SetupPostprocess()
    {
        postprocess.SetActive(true);
        playercamera.gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;

    }
    public void ResetPostprocess()
    {
        postprocess.SetActive(false);
        playercamera.gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;

    }
*/
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