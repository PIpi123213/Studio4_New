using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2End : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject level2end;
    private void Start()
    {
        level2end.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DeadEnd"))
        {
            level2end.SetActive(true);
        }
    }

}
