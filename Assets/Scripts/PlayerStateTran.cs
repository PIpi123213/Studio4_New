using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateTran : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerStateTran Instance { get; private set; }
    private GameObject wingsuitplayer;
    [Header("WingSuit Settings")]
    [Tooltip("初始局部位置")]
    public Vector3 StartLocalPos;
    [Tooltip("初始局部旋转")]
    public Quaternion StartLocalRot;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
     
    }
    public void StageToLevel1()
    {
        StartCoroutine(StageToWingsuit());
        Debug.Log("Stage-Level1");
    }

    private IEnumerator StageToWingsuit()
    {
        SceneTransitionManager.Instance.fadeScreen.FadeOut(0.8f);

        SceneTransitionManager.Instance.GoToSceneAsync("Wingsuit");
        //yield return new WaitForSeconds(1f);

        while (SceneTransitionManager.Instance.CurrentSceneName != "Wingsuit")
        {
            yield return null;
        }
        Debug.LogError("已切换到wingsuit");

        SceneTransitionManager.Instance.fadeScreen.FadeIn(1f);

        wingsuitplayer = GameObject.FindWithTag("wingsuitplayer");
        if (wingsuitplayer == null)
            Debug.LogError("仍然未找到！wingsuitplayer");

        transform.SetParent(wingsuitplayer.transform);
        transform.localPosition = StartLocalPos;
        transform.localRotation = StartLocalRot;

        yield return null;

    }




}
