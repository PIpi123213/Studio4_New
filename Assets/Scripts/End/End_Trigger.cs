using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class End_Trigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private OVRPassthroughLayer Windows_ptLayer;
    [SerializeField] private OVRPassthroughLayer Global_ptLayer;
    public void Awake()
    {
        Windows_ptLayer.textureOpacity = 0f;
        Global_ptLayer.textureOpacity = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("”Œœ∑Ω· ¯");

            StartCoroutine(AnimateOpacity(Global_ptLayer));
        }



    }

    [SerializeField] float opacityDuration = 100f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;

    private void Start()
    {
       
        WindosShowup();
    }
    public void WindosShowup()
    {

        StartCoroutine(AnimateOpacity(Windows_ptLayer));


    }

    private IEnumerator AnimateOpacity(OVRPassthroughLayer ptLayer)
    {

        float elapsedTime = 0f;



        while (elapsedTime < opacityDuration)
        {
            ptLayer.textureOpacity = Mathf.Lerp(startOpacity, endOpacity, elapsedTime / opacityDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        ptLayer.textureOpacity = endOpacity;
      


    }
}
