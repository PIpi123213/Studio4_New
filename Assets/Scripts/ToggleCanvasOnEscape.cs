using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ToggleCanvasOnEscape : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Canvas targetCanvas; // 拖拽你的 Canvas 到 Inspector
    public DynamicMoveProvider dynamicMoveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;
    
 
    void Start()
    {
        //dynamicMoveProvider.enabled = false;
        //turnProvider.enabled = false;
        dynamicMoveProvider.moveSpeed = 0f;
        turnProvider.turnSpeed = 0f;
        targetCanvas.enabled = false;
    }
    void Update()
    {
        // 检测 ESC 按键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
         
            ToggleCanvas();
        }
   
    }
  
    void ToggleCanvas()
    {
        if (targetCanvas != null)
        {
            // 切换 Canvas 的激活状态
            targetCanvas.enabled = !targetCanvas.enabled;

            // 可选：暂停/恢复游戏时间
            //Time.timeScale = targetCanvas.enabled ? 0f : 1f;

            // 可选：锁定/解锁鼠标（适用于菜单界面）
            Cursor.lockState = targetCanvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = targetCanvas.enabled;
        }
        else
        {
            Debug.LogWarning("Target Canvas 未分配！");
        }
    }
    public void lockProvider()
    {
        dynamicMoveProvider.moveSpeed = 0f;
        turnProvider.turnSpeed = 0f;

/*        dynamicMoveProvider.enabled = false;
        turnProvider.enabled = false;*/
        Debug.Log("1111");
    }
    public void UnlockProvider()
    {
        dynamicMoveProvider.moveSpeed = 1f;
        turnProvider.turnSpeed = 60f;

     /*   dynamicMoveProvider.enabled = true;
        turnProvider.enabled = true;*/

    }
}
