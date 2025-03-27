using UnityEngine;

public class RotateObjectWithMouse : MonoBehaviour
{
    public GameObject objectA;  // ����A
    public GameObject objectB;  // ����������B����

    public bool change = false;
    private MoveObjectOnEsc moveObjectScript;  // ����B�Ľű�����

    private Vector3 lastMousePosition;
    private bool isMousePressed = false;
    private Vector3 rotationVelocity = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    public float decelerationTime = 0.5f;

    private void Start()
    {
        // ��ʼ��ʱ��ȡ����B�Ľű�
        if (objectB != null)
        {
            moveObjectScript = objectB.GetComponent<MoveObjectOnEsc>();
            if (moveObjectScript == null)
            {
                Debug.LogWarning("δ������B���ҵ�MoveObjectOnEsc�ű���");
            }
        }
    }

    private void OnEnable()
    {
        EventCenter.Instance.Subscribe("change", OnChangeEventTriggered);
    }

    private void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("change", OnChangeEventTriggered);
    }

    private void OnChangeEventTriggered()
    {
        change = true;
        Debug.Log("��ת������");
    }

    void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // ���������������lookΪtrueʱֱ�ӷ���
        if (!change || (moveObjectScript != null && moveObjectScript.look))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            rotationVelocity = currentVelocity;
        }

        if (isMousePressed && objectA != null)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            float rotateSpeed = 0.1f;
            float rotateX = mouseDelta.y * rotateSpeed;
            float rotateY = -mouseDelta.x * rotateSpeed;

            currentVelocity = new Vector3(rotateX, rotateY, 0);

            objectA.transform.Rotate(Vector3.up, rotateY, Space.World);
            objectA.transform.Rotate(Vector3.right, rotateX, Space.World);
        }

        if (!isMousePressed && rotationVelocity.magnitude > 0)
        {
            float decelerationFactor = Time.deltaTime / decelerationTime;
            rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.zero, decelerationFactor);

            objectA.transform.Rotate(Vector3.up, rotationVelocity.y, Space.World);
            objectA.transform.Rotate(Vector3.right, rotationVelocity.x, Space.World);
        }
    }
}