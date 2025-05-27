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


       

        // �ȴ�������ȫ����
        while (!operation.isDone)
        {
            yield return null;
        }
        operation.allowSceneActivation = true;
        UpdateCurrentScene();
    }
    private AsyncOperation asyncLoad;

    public void StartPreloading(string sceneName)
    {
        StartCoroutine(PreloadScene(sceneName));
    }

    private IEnumerator PreloadScene(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 不立即激活

        while (!asyncLoad.isDone)
        {
            // 这个值最多只会到 0.9，除非你允许激活
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading Progress: " + (progress * 100f) + "%");

            // 当 asyncLoad.progress 到 0.9，表示场景已经加载完毕，只差激活
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Scene is ready to activate.");
                yield break; // 可以在这里等待触发激活
            }

            yield return null;
        }
    }

    public void ActivateScene()
    {
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = true;
        }
    }


}