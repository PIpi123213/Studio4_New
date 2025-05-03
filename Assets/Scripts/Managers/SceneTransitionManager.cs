using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{

    public static            SceneTransitionManager Instance { get; private set; }
    public                   string                 CurrentSceneName;
    [SerializeField] private GameObject             WhiteFadeIn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateCurrentScene();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // MoveManager.Instance.OnSceneIn();//��¼λ��
            //MoveManager.Instance.OnSceneOut();
            /*Scene currentScene = SceneManager.GetActiveScene();
            SceneTransitionManager.Instance.GoToScene(currentScene.name);*/
            GoToScene("New Scene");

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            // MoveManager.Instance.OnSceneIn();//��¼λ��
            //MoveManager.Instance.OnSceneOut();
            /*Scene currentScene = SceneManager.GetActiveScene();
            SceneTransitionManager.Instance.GoToScene(currentScene.name);*/

            StartCoroutine(FadeInAndGoToScene("Climb_Test 1", 2f));


        }



    }
    private IEnumerator FadeInAndGoToScene(string sceneName, float fadeDuration)
    {
        // 运行 FadeWhiteInMaterial 协程
        yield return StartCoroutine(FadeWhiteInMaterial(fadeDuration));

        // 在 FadeWhiteInMaterial 完成后运行 GoToSceneAsync
        yield return StartCoroutine(GoToSceneAsyncRoutine(sceneName));
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
    private IEnumerator FadeWhiteInMaterial(float duration)
    {
        if (WhiteFadeIn == null)
        {
            Debug.LogError("WhiteFadeIn GameObject is not assigned.");
            yield break;
        }

        Renderer renderer = WhiteFadeIn.GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("WhiteFadeIn does not have a Renderer or Material.");
            yield break;
        }

        Material material     = renderer.material;
        Color    initialColor = material.color;
        Color    targetColor  = new Color(initialColor.r, initialColor.g, initialColor.b, 1f); // Alpha = 1 (255)

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            material.color = Color.Lerp(initialColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set
        material.color = targetColor;
    }
}