using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class End_Trigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private OVRPassthroughLayer Windows_ptLayer;
    [SerializeField] private OVRPassthroughLayer Global_ptLayer;
    private bool isEnd = false;
    
    public GameObject Logo;
    public void Awake()
    {
        Windows_ptLayer.textureOpacity = 0f;
        Global_ptLayer.textureOpacity = 0f;
    }

    void Start()
    {
        Logo.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!isEnd)
        {
            Debug.Log("��Ϸ����");

            StartCoroutine(AnimateOpacity(Global_ptLayer));
            isEnd = true;
        }

    }

    [SerializeField] float opacityDuration = 100f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;
    
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

        yield return new WaitForSeconds(2f);
        Logo.SetActive(true);
    }
}
