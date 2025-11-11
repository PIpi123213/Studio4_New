using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanBGMSwitch : MonoBehaviour
{
    [SerializeField] private AudioSource peacefulBGM; // 平和的BGM
    [SerializeField] private AudioSource horrorBGM;   // 恐怖的BGM
    [SerializeField] private float fadeSpeed = 1f;    // 音量渐变速度

    private bool isInHorrorZone = false;             // 是否在恐怖区域
    public bool isTransitioning = false;            // 是否正在切换BGM

    // Start is called before the first frame update
    void Start()
    {
        // 初始化音量
        if (peacefulBGM != null && horrorBGM != null)
        {
            peacefulBGM.volume = 0f;
            horrorBGM.volume = 0f;
            peacefulBGM.Play();
            horrorBGM.Play();
        }
        else
        {
            Debug.LogError("BGM AudioSource未设置！请在Inspector中设置BGM引用。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTransitioning)
        {
            // 根据是否在恐怖区域决定音量渐变方向
            float targetPeacefulVolume = isInHorrorZone ? 0f : 0.5f;
            float targetHorrorVolume = isInHorrorZone ? 0.5f : 0f;

            // 平滑过渡音量
            peacefulBGM.volume = Mathf.MoveTowards(peacefulBGM.volume, targetPeacefulVolume, fadeSpeed * Time.deltaTime);
            horrorBGM.volume = Mathf.MoveTowards(horrorBGM.volume, targetHorrorVolume, fadeSpeed * Time.deltaTime);

            // 检查是否完成过渡
            if (Mathf.Approximately(peacefulBGM.volume, targetPeacefulVolume) &&
                Mathf.Approximately(horrorBGM.volume, targetHorrorVolume))
            {
                isTransitioning = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInHorrorZone = true;
            isTransitioning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInHorrorZone = false;
            isTransitioning = true;
        }
    }

    public void ChangeIsTransition()
    {
        isInHorrorZone = true;
        isTransitioning = true;
    }


    public void toPlay()
    {
        peacefulBGM.volume = 0.5f;
    }
}
