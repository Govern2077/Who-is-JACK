using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    private float rotationSpeed = 10f;  // 控制旋转过渡的速度
    private float transitionTime = 0.2f;  // 旋转到 0 所需的时间
    private MeshRenderer meshRenderer;  // MeshRenderer 组件

    private void Start()
    {
        // 获取物体上的 MeshRenderer 组件
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        // 获取物体当前的旋转角度
        float currentRotationX = transform.rotation.eulerAngles.x;
        float currentRotationY = transform.rotation.eulerAngles.y;

        // 确保旋转角度在 -180 到 180 之间，避免符号问题
        if (currentRotationX > 180f) currentRotationX -= 360f;
        if (currentRotationY > 180f) currentRotationY -= 360f;

        // 如果旋转角度在 -2 到 2 之间，开始平滑过渡到 0
        if (currentRotationX > -3f && currentRotationX < 3f && currentRotationY > -3f && currentRotationY < 3f)
        {
            // 使用 Lerp 平滑过渡到 0
            float targetRotationX = Mathf.Lerp(currentRotationX, 0f, rotationSpeed * Time.deltaTime / transitionTime);
            float targetRotationY = Mathf.Lerp(currentRotationY, 0f, rotationSpeed * Time.deltaTime / transitionTime);

            // 应用新的旋转角度
            transform.rotation = Quaternion.Euler(targetRotationX, targetRotationY, transform.rotation.eulerAngles.z);
        }

        // 判断物体的 rotationX 和 rotationY 是否都为 0
        if (Mathf.Abs(currentRotationX) < 0.1f && Mathf.Abs(currentRotationY) < 0.1f)
        {
            // 如果是，则禁用 MeshRenderer
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
       
    }
}
