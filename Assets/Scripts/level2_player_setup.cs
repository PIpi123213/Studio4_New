using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level2_player_setup : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        this.transform.position = MoveManager.Instance._movementData.Offset;
        //MoveManager.Instance.OnSceneOut();
      
        //SceneTransitionManager.Instance.fadeScreen_Black.FadeIn(1f);
        
    }
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
