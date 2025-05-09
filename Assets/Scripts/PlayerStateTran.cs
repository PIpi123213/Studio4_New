using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateTran : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerStateTran Instance { get; private set; }
    public int Stage = 0;
    public bool isStart = false;

    [Header("Level0 Settings")]
    [Tooltip("level0_scene")]
    public GameObject level0_scene;

    [Tooltip("level0_Drm")]
    public GameObject level0_Drm;


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


    [Header("Level2 Settings")]
    [Tooltip("level2_scene")]
    public GameObject level2_scene;
    [Tooltip("Level2天空盒")]
    public Material skyboxMaterials2;
    [Tooltip("level2_Drm")]
    public GameObject level2_Drm;





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
        /*level1_scene.SetActive(false);
        level2_scene.SetActive(false);*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Level1ToStage2();
        }
    }
    public void restSkybox()
    {
        RenderSettings.skybox = skyboxMaterials;
        RenderSettings.skybox.SetFloat("_Exposure", 0);
    }
    public void ChangeSkyboxLevel2()
    {
        RenderSettings.skybox = skyboxMaterials2;
        RenderSettings.skybox.SetFloat("_Exposure", 0);
    }
    public void Stage_level0()
    {
        level0_Drm.SetActive(true);
        level2_Drm.SetActive(false);
        Stage = 0;
    }
    public void StageToLevel1()
    {
       
        StartCoroutine(StageToWingsuit());
        Debug.Log("Stage-Level1");
    }
    public void Level1ToStage2()
    {
        StartCoroutine(WingsuitToStage());
        Debug.Log("Level1-Level2");
    }

    private IEnumerator StageToWingsuit()
    {
        SceneTransitionManager.Instance.fadeScreen.FadeOut(0.1f);
        yield return new WaitForSeconds(0.3f);
        Coroutine skyboxRoutine = StartCoroutine(AnimateSkyboxExposure(0f, 0.6f, 0.5f));
        
        yield return skyboxRoutine;
        level0_scene.SetActive(false);
        Stage = 1;
        level1_scene.SetActive(true);

        if (wingsuitplayer == null)
            Debug.LogError("仍然未找到！wingsuitplayer");

        MoveManager.Instance.OnSceneIn();
        transform.SetParent(wingsuitplayer.transform);
        transform.localPosition = StartLocalPos;
        transform.localRotation = StartLocalRot;

        yield return new WaitForSeconds(0.5f);
        isStart = true;
        SceneTransitionManager.Instance.fadeScreen.FadeIn(2f);
        yield return null;

    }
    
    private IEnumerator WingsuitToStage()
    {



        //SceneTransitionManager.Instance.fadeScreen_Black.FadeOut(0.8f);
        yield return new WaitForSeconds(0.3f);
        ChangeSkyboxLevel2();
        level0_Drm.SetActive(false);
        level2_Drm.SetActive(true);
        Stage = 2;
        
        transform.SetParent(null);
        yield return null;
        
        this.transform.position = MoveManager.Instance.CurrentWorldPosition + level2_scene.transform.position; 
        //MoveManager.Instance.OnSceneOut();
        yield return new WaitForSeconds(0.5f);
        SceneTransitionManager.Instance.fadeScreen_Black.FadeIn(1f);
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
