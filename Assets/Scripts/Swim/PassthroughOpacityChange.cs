using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughOpacityChange : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer ptLayer;
    
    /*
    [SerializeField] private float opacityDuration = 5f;
    */
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity   = 0f;
    
    public void StartChangePasssthroughOpacity(float opacityDuration)
    {
        if (ptLayer == null)
        {
            Debug.LogError("OVRPassthroughLayer is not assigned!");
            return;
        }
        
        ptLayer.textureOpacity = startOpacity; // 设置初始透明度
        StartCoroutine(AnimateOpacity(opacityDuration));
    }
    
    private IEnumerator AnimateOpacity(float opacityDuration)
    {
        float elapsedTime = 0f;
        Debug.Log("AnimateOpacity");
        
        while (elapsedTime < opacityDuration)
        {
            ptLayer.textureOpacity =  Mathf.Lerp(startOpacity, endOpacity, elapsedTime / opacityDuration);
            elapsedTime            += Time.deltaTime;
            yield return null;
        }
        ptLayer.textureOpacity = endOpacity;
    }
}
