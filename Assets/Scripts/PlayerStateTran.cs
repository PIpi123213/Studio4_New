using OVR.OpenVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerStateTran : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerStateTran Instance { get; private set; }
    public int Stage = 0;
    public bool isStart = false;
    public Camera playercamera = null;


    [Header("Level1 Settings")]
    [Tooltip("level1_scene")]
    public GameObject level1_scene;
    [Tooltip("wingsuitplayer")]
    public GameObject wingsuitplayer;
    [Tooltip("初始局部位置")]
    public Vector3 StartLocalPos;
    [Tooltip("初始局部旋转")]
    public Quaternion StartLocalRot;
    [Tooltip("Level1天空盒")]
    public Material skyboxMaterials;

    private UniversalAdditionalCameraData cameraData;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        restSkybox();
    }
    private void Start()
    {
        //level0_scene.SetActive(true);
        level1_scene.SetActive(false);
     
        cameraData = playercamera.GetUniversalAdditionalCameraData();
        cameraData.renderPostProcessing = false;
        cameraData.renderPostProcessing = false;
        RenderSettings.fog = false;
    }

    private void Update()
    {
    
       
        
    }
        public void restSkybox()
    {
        RenderSettings.skybox = skyboxMaterials;
        RenderSettings.skybox.SetFloat("_Exposure", 0);
    }
 
    public void Stage_level0()
    {
       
        Stage = 0;
    }

    public void pretoOcean()
    {
        level1_scene.SetActive(true);

    }


    public void StageToLevel1()
    {
        
        StartCoroutine(StageToOcean());
        Debug.Log("Stage-Level1");
    }
    public void Level1ToStage2()
    {
        StartCoroutine(WingsuitToStage());
        Debug.Log("Level1-Level2");
    }
    private IEnumerator StageToOcean()
    {
       // playercamera.clearFlags = CameraClearFlags.Skybox;

        //SceneTransitionManager.Instance.fadeScreen.FadeOut(0.1f);
     




        yield return new WaitForSeconds(0.2f);
        Stage = 1;
        RenderSettings.fog = true;
        cameraData.renderPostProcessing = true;
        yield return new WaitForSeconds(0.3f);

       // SceneTransitionManager.Instance.fadeScreen.FadeIn(0.2f);

       

        if (wingsuitplayer == null)
            Debug.LogError("仍然未找到！wingsuitplayer");

        Vector3 targetWorldPos = wingsuitplayer.transform.position ;
        //Quaternion aRotation = playerTransform.rotation;
        Vector3 worldOffsetFromAtoB = this.transform.TransformPoint(MoveManager.Instance.TrackingObject.localPosition) - this.transform.position;
        Vector3 desiredAWorldPos = targetWorldPos - worldOffsetFromAtoB;
        // sliderPlayerposition = sliderPlayerposition - (aRotation*new Vector3(MoveManager.Instance.TrackingObject.transform.localPosition.x , 0f, MoveManager.Instance.TrackingObject.transform.localPosition.z)) ;

        this.transform.position = desiredAWorldPos;
        transform.SetParent(wingsuitplayer.transform);

        StartLocalRot.x = 0f;
        StartLocalRot.z = 0f;
        float targetY = -MoveManager.Instance.TrackingObject.localRotation.eulerAngles.y;
        //StartLocalRot.y = -MoveManager.Instance.TrackingObject.localRotation.y;
        //Debug.Log(StartLocalRot.y);
        //Debug.Log(-MoveManager.Instance.TrackingObject.localRotation.y);
        transform.localRotation = Quaternion.Euler(StartLocalRot.x, 0f, StartLocalRot.z);

        //transform.localPosition = StartLocalPos;
        // transform.localRotation = StartLocalRot;

        yield return new WaitForSeconds(1f);

        SceneTransitionManager.Instance.fadeScreen.FadeIn(3f);
        yield return null;

    }
    private IEnumerator StageToWingsuit()
    {
        playercamera.clearFlags = CameraClearFlags.Skybox;
      
        SceneTransitionManager.Instance.fadeScreen.FadeOut(0.1f);
        yield return new WaitForSeconds(0.3f);
       Coroutine skyboxRoutine = StartCoroutine(AnimateSkyboxExposure(0f, 0.6f, 0.5f));
        
        yield return skyboxRoutine;
       
        Stage = 1;
        
       
        if (wingsuitplayer == null)
            Debug.LogError("仍然未找到！wingsuitplayer");

        Vector3 targetWorldPos = wingsuitplayer.transform.position ;
        //Quaternion aRotation = playerTransform.rotation;
        Vector3 worldOffsetFromAtoB =this.transform.TransformPoint(MoveManager.Instance.TrackingObject.localPosition) - this.transform.position;
        Vector3 desiredAWorldPos = targetWorldPos - worldOffsetFromAtoB;
        // sliderPlayerposition = sliderPlayerposition - (aRotation*new Vector3(MoveManager.Instance.TrackingObject.transform.localPosition.x , 0f, MoveManager.Instance.TrackingObject.transform.localPosition.z)) ;

        this.transform.position = desiredAWorldPos;

       
        transform.SetParent(wingsuitplayer.transform);

     /*   StartLocalRot.x = 0f;
        StartLocalRot.z = 0f;
        float targetY = -MoveManager.Instance.TrackingObject.localRotation.eulerAngles.y;*/
        //StartLocalRot.y = -MoveManager.Instance.TrackingObject.localRotation.y;
        //Debug.Log(StartLocalRot.y);
        //Debug.Log(-MoveManager.Instance.TrackingObject.localRotation.y);
       // transform.localRotation = Quaternion.Euler(StartLocalRot.x, targetY, StartLocalRot.z);

        //transform.localPosition = StartLocalPos;
        // transform.localRotation = StartLocalRot;

      
        
        SceneTransitionManager.Instance.fadeScreen.FadeIn(3f);
        yield return null;

    }
    
    private IEnumerator WingsuitToStage()
    {


        //level1_scene.SetActive(false);
       
        //level2_scene.SetActive(true);
        SceneTransitionManager.Instance.fadeScreen_Black.FadeOut(0.8f);
        //yield return new WaitForSeconds(0.3f);

        //ChangeSkyboxLevel2();

        //level2_Drm.SetActive(true);
        Stage = 2;

        SceneTransitionManager.Instance.GoToSceneAsync("New Scene"); 
        //SceneTransitionManager.Instance.GoToScene("New Scene");
        yield return null;
        
       

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


}
