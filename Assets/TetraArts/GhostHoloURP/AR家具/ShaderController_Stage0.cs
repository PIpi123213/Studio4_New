using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController_Stage0 : MonoBehaviour
{
    // Start is called before the first frame update
      public float startValue = 1.2f;
    public float endValue = -0.87f;
    public float duration = 2f;

    [Header("Door")]
    public GameObject Door;
    public Material doorMaterial;
    [Header("desk")]
    public GameObject Desk;
    public Material deskMaterial;
 
    [Header("sofa")]
    public GameObject sofaConner;
    public Material sofaMaterial;
    [Header("shelf")]
    public GameObject bookShelf;
    public Material bookshelfMaterial;

    [Header("shelving")]
    public GameObject shelving;
    public Material shelvingMaterial;

    [Header("Letter")]
    public GameObject letter;

    void Start()
    {
       // initalMaterial(doorMaterial);
        initalMaterial(deskMaterial);
        initalMaterial(sofaMaterial);
        initalMaterial(bookshelfMaterial);
        initalMaterial(shelvingMaterial);
        letter.SetActive(false);
    }

    private void Update()
    {
 
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
    private IEnumerator GradientTransition_Out(Material targetMaterial)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentValue = Mathf.Lerp( endValue, startValue, elapsedTime / duration);
            targetMaterial.SetFloat("_GradientPos", currentValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set
        targetMaterial.SetFloat("_GradientPos", startValue);
    }

    public void initalMaterial(Material targetMaterial)
    {
        targetMaterial.SetFloat("_GradientPos", startValue);

    }

    public void Startgame()
    {
        StartCoroutine(StartGameShowUp());
    }

    private IEnumerator StartGameShowUp()
    {
        StartCoroutine(GradientTransition(sofaMaterial));
        StartCoroutine(GradientTransition(bookshelfMaterial));
        StartCoroutine(GradientTransition(deskMaterial));
        StartCoroutine(GradientTransition(shelvingMaterial));
        yield return new WaitForSeconds(duration+2f);

        StartCoroutine(GradientTransition_Out(sofaMaterial));
        StartCoroutine(GradientTransition_Out(bookshelfMaterial));
        StartCoroutine(GradientTransition_Out(shelvingMaterial));
        yield return new WaitForSeconds(duration);
        sofaConner.SetActive(false);
        bookShelf.SetActive(false);
        shelving.SetActive(false);
        letter.SetActive(true);

        yield return null;
    }

    public void ChangeDeskMaterial_dissapear()
    {
        StartCoroutine(deskDissapear());

    }

    private IEnumerator deskDissapear()
    {
        StartCoroutine(GradientTransition_Out(deskMaterial));
        StartCoroutine(GradientTransition_Out(doorMaterial));
        yield return new WaitForSeconds(duration);
        Desk.SetActive(false);
        Door.SetActive(false);
        yield return null;
    }

}
