using UnityEngine;

public class SkyboxBrightnessChanger : MonoBehaviour
{
    public float targetExposure  = 0.5f; // 目标曝光度（暗）
    public float transitionSpeed = 1f;   // 变化速度
    public bool  resetOnExit     = true; // 玩家离开时是否恢复原曝光

    private float originalExposure;
    private bool  isPlayerInside = false;

    void Start()
    {
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            originalExposure = RenderSettings.skybox.GetFloat("_Exposure");
        }
        else
        {
            Debug.LogWarning("Skybox material does not have _Exposure property!");
        }
    }

    void Update()
    {
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            float currentExposure = RenderSettings.skybox.GetFloat("_Exposure");
            float target          = isPlayerInside ? targetExposure : originalExposure;
            float newExposure     = Mathf.Lerp(currentExposure, target, Time.deltaTime * transitionSpeed);
            RenderSettings.skybox.SetFloat("_Exposure", newExposure);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && resetOnExit)
        {
            isPlayerInside = false;
        }
    }
}