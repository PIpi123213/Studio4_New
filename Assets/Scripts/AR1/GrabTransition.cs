using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;

public class GrabTransition : MonoBehaviour
{
    // Start is called before the first frame update
    public GrabHandPose handPoseGrab;
    private bool istrigger = false;


    //public Camera playercamera;
    //public GameObject scene;
    public PlayableDirector falldowntimeline;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (handPoseGrab.HandGrabing==2&&!istrigger)
        {
            istrigger = true;
            //SceneTransitionManager.Instance.fadeScreen.FadeOut(0.1f);
            trantoFly();
        }
    }

    public void trantoFly()
    {
        /*  audioSource.Play();
          PlayerStateTran.Instance.pretoOcean();*/

        falldowntimeline.Play(); 


        //PlayerStateTran.Instance.StageToLevel1();
   

    }
   /* public IEnumerator AnimateSkyboxExposure(float startExposure, float endExposure, float duration)
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
    }*/
}
