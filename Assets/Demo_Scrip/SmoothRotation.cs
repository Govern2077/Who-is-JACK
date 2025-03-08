using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    private float rotationSpeed = 10f;  // ������ת���ɵ��ٶ�
    private float transitionTime = 1f;  // ��ת�� 0 �����ʱ��
    private MeshRenderer meshRenderer;  // ��ǰ����� MeshRenderer ���
    public GameObject objectA;  // ���� A

    private void Start()
    {
        // ��ȡ�����ϵ� MeshRenderer ���
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        // �����ǰ����� MeshRenderer ����ǹرյģ�ֱ�ӷ���
        if (meshRenderer != null && !meshRenderer.enabled)
        {
            return;
        }

        // ��ȡ���嵱ǰ����ת�Ƕ�
        float currentRotationX = transform.rotation.eulerAngles.x;
        float currentRotationY = transform.rotation.eulerAngles.y;

        // ȷ����ת�Ƕ��� -180 �� 180 ֮�䣬�����������
        if (currentRotationX > 180f) currentRotationX -= 360f;
        if (currentRotationY > 180f) currentRotationY -= 360f;

        // �����ת�Ƕ��� -2 �� 2 ֮�䣬��ʼƽ�����ɵ� 0
        if (currentRotationX > -5f && currentRotationX < 5f && currentRotationY > -5f && currentRotationY < 5f)
        {
            // ����һ��Ŀ����ת�Ƕ�
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z);

            // ʹ�� Slerp ƽ�����ɵ�Ŀ����ת
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / transitionTime);
        }

        // �ж������ rotationX �� rotationY �Ƿ�Ϊ 0
        if (Mathf.Abs(currentRotationX) < 2f && Mathf.Abs(currentRotationY) < 2f)
        {
            // ����ǣ�����õ�ǰ����� MeshRenderer
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }

            // ͬʱ�������� A �� MeshRenderer
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
