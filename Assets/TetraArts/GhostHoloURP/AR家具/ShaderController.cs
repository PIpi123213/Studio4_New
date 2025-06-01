using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    public float startValue = 1.2f;
    public float endValue = -0.87f;
    public float duration = 2f;

    public GameObject sofaConner;
    public GameObject bookShelf;
    
    public Material shelfMaterial;
    public Material sofaMaterial;
    public Material bookshelfMaterial;

    void Start()
    {
        sofaConner.SetActive(false);
        bookShelf.SetActive(false);
        
        if (shelfMaterial != null)
        {
            StartCoroutine(GradientTransition(shelfMaterial));
        }
        else
        {
            Debug.LogWarning("Ghost Holo Material is not assigned!");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ChangeSofaMaterial();
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            ChangeBookShelfMaterial();
        }
    }

    public void ChangeSofaMaterial()
    {
        sofaConner.SetActive(true);
        StartCoroutine(GradientTransition(sofaMaterial));
    }
    
    public void ChangeBookShelfMaterial()
    {
        bookShelf.SetActive(true);
        StartCoroutine(GradientTransition(bookshelfMaterial));
    }
    
    private IEnumerator GradientTransition(Material targetMaterial)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            targetMaterial.SetFloat("_GradientPos", currentValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set
        targetMaterial.SetFloat("_GradientPos", endValue);
    }
    

}
