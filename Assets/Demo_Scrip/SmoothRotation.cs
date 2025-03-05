using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    private float rotationSpeed = 10f;  // ������ת���ɵ��ٶ�
    private float transitionTime = 0.2f;  // ��ת�� 0 �����ʱ��
    private MeshRenderer meshRenderer;  // MeshRenderer ���

    private void Start()
    {
        // ��ȡ�����ϵ� MeshRenderer ���
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        // ��ȡ���嵱ǰ����ת�Ƕ�
        float currentRotationX = transform.rotation.eulerAngles.x;
        float currentRotationY = transform.rotation.eulerAngles.y;

        // ȷ����ת�Ƕ��� -180 �� 180 ֮�䣬�����������
        if (currentRotationX > 180f) currentRotationX -= 360f;
        if (currentRotationY > 180f) currentRotationY -= 360f;

        // �����ת�Ƕ��� -2 �� 2 ֮�䣬��ʼƽ�����ɵ� 0
        if (currentRotationX > -3f && currentRotationX < 3f && currentRotationY > -3f && currentRotationY < 3f)
        {
            // ʹ�� Lerp ƽ�����ɵ� 0
            float targetRotationX = Mathf.Lerp(currentRotationX, 0f, rotationSpeed * Time.deltaTime / transitionTime);
            float targetRotationY = Mathf.Lerp(currentRotationY, 0f, rotationSpeed * Time.deltaTime / transitionTime);

            // Ӧ���µ���ת�Ƕ�
            transform.rotation = Quaternion.Euler(targetRotationX, targetRotationY, transform.rotation.eulerAngles.z);
        }

        // �ж������ rotationX �� rotationY �Ƿ�Ϊ 0
        if (Mathf.Abs(currentRotationX) < 0.1f && Mathf.Abs(currentRotationY) < 0.1f)
        {
            // ����ǣ������ MeshRenderer
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
       
    }
}
