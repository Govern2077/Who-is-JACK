using UnityEngine;

public class Threadswitching: MonoBehaviour
{
    [Header("目标物体列表")]
    public GameObject[] targetObjects; // 在Inspector中拖拽赋值

    private MeshRenderer ownRenderer;

    void Start()
    {
        // 缓存自己的MeshRenderer组件
        ownRenderer = GetComponent<MeshRenderer>();

        // 安全校验
        if (ownRenderer == null)
        {
            Debug.LogError("找不到自身的Mesh Renderer组件！", this);
            enabled = false;
        }
    }

    void Update()
    {
        bool shouldEnable = false;

        // 遍历所有目标物体
        foreach (GameObject target in targetObjects)
        {
            if (target == null || !target.activeInHierarchy) continue;

            MeshRenderer targetRenderer = target.GetComponent<MeshRenderer>();

            // 如果目标物体没有Mesh Renderer则跳过
            if (targetRenderer == null)
            {
                Debug.LogWarning($"目标物体 {target.name} 没有Mesh Renderer组件", this);
                continue;
            }

            // 发现任意一个开启的Render就激活自身
            if (targetRenderer.enabled)
            {
                shouldEnable = true;
                break; // 发现开启的组件后立即跳出循环
            }
        }

        // 设置自身Render状态
        ownRenderer.enabled = shouldEnable;
    }

    // 编辑器中的安全校验
    void OnValidate()
    {
        if (targetObjects == null || targetObjects.Length == 0)
        {
            Debug.LogWarning("请至少添加一个目标物体到列表中！", this);
        }
    }
}