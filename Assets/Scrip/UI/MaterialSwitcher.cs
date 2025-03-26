using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialSwitcher : MonoBehaviour
{
    [Header("目标物体设置")]
    [Tooltip("需要检测的MeshRenderer")]
    public Renderer targetRenderer;  // 需要拖拽指定目标物体的MeshRenderer

    [Header("材质设置")]
    [Tooltip("要切换的新材质")]
    public Material newMaterial;     // 需要拖拽指定新材质

    private Renderer myRenderer;
    private bool hasSwitched = false; // 是否已经切换过材质
    private bool targetHasBeenEnabled = false; // 记录目标是否被启用过

    void Start()
    {
        // 获取自身Renderer组件
        myRenderer = GetComponent<Renderer>();

        // 安全验证
        if (targetRenderer == null)
        {
            Debug.LogWarning("目标物体未指定！", this);
            enabled = false;
        }
    }

    void Update()
    {
        // 如果目标物体当前处于激活状态
        if (targetRenderer.enabled)
        {
            targetHasBeenEnabled = true; // 标记目标物体曾经被启用过
        }

        // 当检测到目标物体被启用过且未执行过切换
        if (targetHasBeenEnabled && !hasSwitched)
        {
            SwitchMaterial();
        }
    }

    void SwitchMaterial()
    {
        // 切换材质
        if (newMaterial != null)
        {
            myRenderer.material = newMaterial;
            Debug.Log("材质已切换", this);

            // 标记已切换
            hasSwitched = true;

            // 可选：禁用脚本后续运行
            // enabled = false;
        }
        else
        {
            Debug.LogWarning("新材质未指定！", this);
        }
    }
}