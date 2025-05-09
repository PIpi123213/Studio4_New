using System.Collections;
using UnityEngine;

public class RandomCameraLight : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    // 最小和最大间隔时间
    [SerializeField] private float minInterval = 0.1f;
    [SerializeField] private float maxInterval = 0.5f;

    void Start()
    {
        // 获取 MeshRenderer 组件
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("No MeshRenderer component found on this GameObject!");
            return;
        }

        // 启动闪烁协程
        StartCoroutine(FlashMesh());
    }

    private IEnumerator FlashMesh()
    {
        while (true)
        {
            // 使 MeshRenderer 启用
            meshRenderer.enabled = true;

            // 保持亮起 0.1 秒
            yield return new WaitForSeconds(0.1f);

            // 使 MeshRenderer 禁用
            meshRenderer.enabled = false;

            // 随机生成间隔时间
            float interval = Random.Range(minInterval, maxInterval);

            // 等待随机时间
            yield return new WaitForSeconds(interval);
        }
    }
}