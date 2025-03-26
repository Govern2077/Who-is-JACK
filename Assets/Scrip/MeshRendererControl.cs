using UnityEngine;

public class MeshRendererControl : MonoBehaviour
{
    public GameObject objectA; // 用来拖拽物体A
    public GameObject objectC; // 用来拖拽物体C
    private MeshRenderer meshRenderer; // 当前物体的 MeshRenderer
    private RotateObjectWithMouse rotateScript; // 物体A上的 RotateObjectWithMouse 脚本
    private MeshRenderer meshRendererC; // 物体C的 MeshRenderer

    void Start()
    {
        // 获取当前物体的 MeshRenderer
        meshRenderer = GetComponent<MeshRenderer>();

        // 获取物体A上的 RotateObjectWithMouse 脚本
        if (objectA != null)
        {
            rotateScript = objectA.GetComponent<RotateObjectWithMouse>();
        }

        // 获取物体C的 MeshRenderer
        if (objectC != null)
        {
            meshRendererC = objectC.GetComponent<MeshRenderer>();
        }

        // 初始化时根据条件设置 MeshRenderer 的状态
        UpdateMeshRendererStatus();
    }

    void Update()
    {
        // 每帧检查并更新 MeshRenderer 状态
        UpdateMeshRendererStatus();
    }

    void UpdateMeshRendererStatus()
    {
        if (meshRendererC != null && rotateScript != null)
        {
            // 如果物体C的 MeshRenderer 打开，并且 RotateObjectWithMouse 脚本中的 change 为 true
            if (meshRendererC.enabled && rotateScript.change)
            {
                // 启用当前物体的 MeshRenderer
                meshRenderer.enabled = true;
            }
            else
            {
                // 否则关闭当前物体的 MeshRenderer
                meshRenderer.enabled = false;
            }
        }
        else
        {
            // 如果找不到脚本或物体C的 MeshRenderer，则关闭当前物体的 MeshRenderer
            meshRenderer.enabled = false;
        }
    }
}
