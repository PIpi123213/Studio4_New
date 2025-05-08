using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateTran : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerStateTran Instance { get; private set; }
    public int Stage = 0;


    [Header("Level0 Settings")]
    [Tooltip("level0_scene")]
    public GameObject level0_scene;



    [Header("Level1 Settings")]
    [Tooltip("level1_scene")]
    public GameObject level1_scene;
    [Tooltip("wingsuitplayer")]
    public GameObject wingsuitplayer;
    [Tooltip("初始局部位置")]
    public Vector3 StartLocalPos;
    [Tooltip("初始局部旋转")]
    public Quaternion StartLocalRot;

    [Header("Level2 Settings")]
    [Tooltip("level2_scene")]
    public GameObject level2_scene;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
     
    }
    private void Start()
    {
        level0_scene.SetActive(true);
        level1_scene.SetActive(false);
        level2_scene.SetActive(false);
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
        SceneTransitionManager.Instance.fadeScreen.FadeOut(0.8f);
        yield return new WaitForSeconds(0.5f);

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
        SceneTransitionManager.Instance.fadeScreen.FadeIn(2f);
        yield return null;

    }
    private IEnumerator WingsuitToStage()
    {
        SceneTransitionManager.Instance.fadeScreen.FadeOut(0.8f);
        yield return new WaitForSeconds(0.5f);

        level1_scene.SetActive(false);
        Stage = 2;
        level2_scene.SetActive(true);
        transform.SetParent(null);
        yield return null;
        MoveManager.Instance.OnSceneOut();
        yield return new WaitForSeconds(0.5f);
        SceneTransitionManager.Instance.fadeScreen.FadeIn(1f);
        yield return null;

    }



}
