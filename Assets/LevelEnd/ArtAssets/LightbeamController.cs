using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightbeamController : MonoBehaviour
{
    public GameObject Lightbeam;
    
    public Material LightbeamMaterial; // 光束材质
    
    public float StartTweak = 1.5f; 
    public float EndTweak = 0.7f;
    public float TweakTransitionDuration = 2f; // 过渡时间
    
    void Start()
    {
       // Lightbeam.SetActive(false);
    }
    
    public void ChangeLightbeamTweak()
    {
        if (LightbeamMaterial != null)
        {
            Lightbeam.SetActive(true); // 激活光束
            StartCoroutine(TweakTransition(LightbeamMaterial));
        }
        else
        {
            Debug.LogWarning("Lightbeam Material is not assigned!");
        }
    }
    
    private IEnumerator TweakTransition(Material targetMaterial)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < TweakTransitionDuration)
        {
            float t = elapsedTime / TweakTransitionDuration;
            float currentTweak = Mathf.Lerp(StartTweak, EndTweak, t);
            targetMaterial.SetFloat("_Tweak", currentTweak); // 假设材质有一个名为 "_Tweak" 的属性
            
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }
        
        // 确保最终值设置正确
        targetMaterial.SetFloat("_Tweak", EndTweak);
    }
}
