using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndGrabtrigger : MonoBehaviour
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
        if (handPoseGrab.HandGrabing !=0 && !istrigger)
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
}
