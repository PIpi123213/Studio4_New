using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTransition : MonoBehaviour
{
    // Start is called before the first frame update
    public GrabHandPose handPoseGrab;
    private bool istrigger = false;
    public FadeScreen fadeScreen;
    [SerializeField] private AudioSource audioSource; 
    //public Camera playercamera;
    //public GameObject scene;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (handPoseGrab.HandGrabing==2&&!istrigger)
        {
            istrigger = true;
            trantoFly();
        }
    }

    public void trantoFly()
    {
        audioSource.Play();
        PlayerStateTran.Instance.StageToLevel1();
   

    }
    public IEnumerator AnimateSkyboxExposure(float startExposure, float endExposure, float duration)
    {
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float exposure = Mathf.Lerp(startExposure, endExposure, elapsedTime / duration);
                RenderSettings.skybox.SetFloat("_Exposure", exposure);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // ȷ�������ع��ΪĿ��ֵ
            RenderSettings.skybox.SetFloat("_Exposure", endExposure);
        }
    }
}
