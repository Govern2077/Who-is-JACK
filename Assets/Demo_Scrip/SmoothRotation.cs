using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    private float rotationSpeed = 10f;  // 控制旋转过渡的速度
    private float transitionTime = 1f;  // 旋转到 0 所需的时间
    private MeshRenderer meshRenderer;  // 当前物体的 MeshRenderer 组件
    public GameObject objectA;  // 物体 A

    // 引用另一个物体的 RightClickEffect 脚本
    public RightClickEffect rightClickEffect;  // 引用 RightClickEffect 脚本

    // 引用物体 C
    public GameObject objectC;  // 物体 C

    private void Start()
    {
        // 获取物体上的 MeshRenderer 组件
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        // 如果当前物体的 MeshRenderer 组件是关闭的，直接返回
        if (meshRenderer != null && !meshRenderer.enabled)
        {
            return;
        }

        // 获取物体 C 的旋转角度
        float currentRotationX = objectC.transform.rotation.eulerAngles.x;
        float currentRotationY = objectC.transform.rotation.eulerAngles.y;

        // 确保旋转角度在 -180 到 180 之间，避免符号问题
        if (currentRotationX > 180f) currentRotationX -= 360f;
        if (currentRotationY > 180f) currentRotationY -= 360f;

        // 如果旋转角度在 -2 到 2 之间，开始平滑过渡到 0
        if (currentRotationX > -5f && currentRotationX < 5f && currentRotationY > -5f && currentRotationY < 5f)
        {
            // 创建一个目标旋转角度
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, objectC.transform.rotation.eulerAngles.z);

            // 使用 Slerp 平滑过渡到目标旋转
            objectC.transform.rotation = Quaternion.Slerp(objectC.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / transitionTime);
        }

        // 判断物体 C 的 rotationX 和 rotationY 是否都为 0，并且 RightClickEffect 脚本中的 white 为 true
        if (Mathf.Abs(currentRotationX) < 0.1f && Mathf.Abs(currentRotationY) < 0.1f && rightClickEffect != null && rightClickEffect.white)
        {
            // 如果是，则禁用当前物体的 MeshRenderer
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }

            // 同时启用物体 A 的 MeshRenderer
            if (objectA != null)
            {
                MeshRenderer objectAMeshRenderer = objectA.GetComponent<MeshRenderer>();
                if (objectAMeshRenderer != null)
                {
                    objectAMeshRenderer.enabled = true;
                }
            }
        }
    }
}
