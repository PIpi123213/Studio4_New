using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{

    public static SceneTransitionManager Instance { get; private set; }
    public        string                 CurrentSceneName;
    public        FadeScreen             fadeScreen;
    public FadeScreen fadeScreen_Black;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
        UpdateCurrentScene();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            Scene currentScene = SceneManager.GetActiveScene();
            SceneTransitionManager.Instance.GoToScene(currentScene.name);
            //GoToScene("New Scene");

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            
            StartCoroutine(FadeOutAndGoToSceneRoutine("TheLastPlay"));


        }



    }

    private void UpdateCurrentScene()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Current Scene: {CurrentSceneName}");
    }
    // ͬ����������
    public void GoToScene(string sceneIndex)
    {

        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    private IEnumerator GoToSceneRoutine(string sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        yield return null;
        UpdateCurrentScene();

    }
    private IEnumerator FadeOutAndGoToSceneRoutine(string sceneIndex)
    {
        fadeScreen.FadeOut(fadeScreen.FadeDuration);
        yield return new WaitForSeconds(fadeScreen.FadeDuration);

        SceneManager.LoadScene(sceneIndex);
        yield return null;
        UpdateCurrentScene();

    }

    // �첽�������أ������ȿ��ƣ�
    public void GoToSceneAsync(string sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    private IEnumerator GoToSceneAsyncRoutine(string sceneIndex)
    {


        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float timer = 0;


        operation.allowSceneActivation = true;

        // �ȴ�������ȫ����
        while (!operation.isDone)
        {
            yield return null;
        }

        UpdateCurrentScene();
    }

  

}