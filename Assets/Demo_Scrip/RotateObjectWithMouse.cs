using UnityEngine;

public class RotateObjectWithMouse : MonoBehaviour
{
    // ������A����Ϊpublic����
    public GameObject objectA;  // ����A

    private Vector3 lastMousePosition;
    private bool isMousePressed = false;

    // ���ڼ�¼��ת�ٶ�
    private Vector3 rotationVelocity = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;

    // ���ڿ���˥��ʱ�����ת�ٶ�
    public float decelerationTime = 0.5f;  // ��ת����ʱ��

    private void Update()
    {
        // ����������Ƿ���
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            lastMousePosition = Input.mousePosition; // ��¼����ʱ�����λ��
        }

        // ����������Ƿ��ɿ�
        if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            // ���ɿ����ʱ��¼��ת�ٶ�
            rotationVelocity = currentVelocity;
        }

        // �����갴�£�������ת
        if (isMousePressed && objectA != null)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition; // ��������λ��
            lastMousePosition = Input.mousePosition; // �������λ��

            // �����λ��ת��Ϊ��ת�Ƕ�
            float rotateSpeed = 0.1f; // ��ת�ٶ�ϵ��
            float rotateX = mouseDelta.y * rotateSpeed;
            float rotateY = -mouseDelta.x * rotateSpeed;

            // ���µ�ǰ����ת�ٶ�
            currentVelocity = new Vector3(rotateX, rotateY, 0);

            // ��ת����A
            objectA.transform.Rotate(Vector3.up, rotateY, Space.World); // ��Y����ת
            objectA.transform.Rotate(Vector3.right, rotateX, Space.World); // ��X����ת
        }

        // �������ɿ�����ת��������
        if (!isMousePressed && rotationVelocity.magnitude > 0)
        {
            // �𽥼�����ת�ٶ�
            float decelerationFactor = Time.deltaTime / decelerationTime;
            rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.zero, decelerationFactor);

            // ������ת�ٶȼ�����ת
            objectA.transform.Rotate(Vector3.up, rotationVelocity.y, Space.World);
            objectA.transform.Rotate(Vector3.right, rotationVelocity.x, Space.World);
        }
    }
}
