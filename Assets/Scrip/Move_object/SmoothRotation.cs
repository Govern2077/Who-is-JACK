using UnityEngine;
using System.Collections;

public class SmoothRotation : MonoBehaviour
{
    [Header("Alignment Settings")]
    public Vector3 alignmentVector = Vector3.back;  // ��������ϵ���뷽�򣨿���Inspector�޸ģ�
    public float maxAngle = 2f;                     // ��������ĽǶ���ֵ���ȣ�
    public float rotationDuration = 0.2f;           // �������ʱ�䣨�룩

    [Header("Visualization")]
    public Color objectVectorColor = Color.blue;    // ����������ɫ
    public Color targetVectorColor = Color.red;     // Ŀ��������ɫ
    public float vectorLength = 1f;                 // ���ӻ���������

    [Header("References")]
    public GameObject objectA;
    public RightClickEffect rightClickEffect;
    public GameObject objectC;

    private MeshRenderer meshRenderer;
    private Quaternion targetRotation;
    private bool isRotating;
    private float rotationStartTime;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        EventCenter.Instance.Subscribe("white", OnWhiteEventTriggered);
    }

    private void Update()
    {
        if (meshRenderer != null && !meshRenderer.enabled) return;

        // ��ȡ����C�ı��ض���������ת��Ϊ��������ϵ��
        Vector3 worldAlignment = objectC.transform.TransformDirection(alignmentVector.normalized);

        // ������Ŀ��������0,0,-1���ļн�
        float angle = Vector3.Angle(worldAlignment, Vector3.back);

        // ���Ƕ�С����ֵ����������ʱ��ʼ����
        if (angle <= maxAngle &&
            rightClickEffect != null &&
            rightClickEffect.white &&
            !rightClickEffect.isChanging &&
            !isRotating&& !Input.GetMouseButton(0))
        {
            StartRotationAlignment(worldAlignment);
        }

        // ִ����ת��ֵ
        if (isRotating)
        {
            float progress = (Time.time - rotationStartTime) / rotationDuration;
            objectC.transform.rotation = Quaternion.Slerp(
                objectC.transform.rotation,
                targetRotation,
                Mathf.Clamp01(progress)
            );

            // ��ת��ɼ�⣨�Ƕ�С��0.1�ȣ�
            if (progress >= 0.999f)
            {
                isRotating = false;
                OnAlignmentComplete();
            }
        }
    }

    // ��ʼ����ת����
    private void StartRotationAlignment(Vector3 currentDirection)
    {
        // ����ӵ�ǰ����Ŀ�귽�����ת
        targetRotation = Quaternion.FromToRotation(currentDirection, Vector3.back) * objectC.transform.rotation;
        isRotating = true;
        rotationStartTime = Time.time;
    }

    // ������ɴ���
    private void OnAlignmentComplete()
    {
        EventCenter.Instance.TriggerEvent("white");
        StartCoroutine(DelayedAction());
    }

    // ���ӻ�����
    private void OnDrawGizmos()
    {
        if (objectC == null) return;

        // �������������������ɫ��
        Gizmos.color = objectVectorColor;
        Vector3 worldStart = objectC.transform.position;
        Vector3 worldAlignment = objectC.transform.TransformDirection(alignmentVector.normalized);
        Gizmos.DrawLine(worldStart, worldStart + worldAlignment * vectorLength);

        // ����Ŀ����������ɫ��
        Gizmos.color = targetVectorColor;
        Gizmos.DrawLine(worldStart, worldStart + Vector3.back * vectorLength);
    }

    private IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(0.03f);

        if (meshRenderer != null) meshRenderer.enabled = false;

        if (objectA != null)
        {
            MeshRenderer aRenderer = objectA.GetComponent<MeshRenderer>();
            if (aRenderer != null) aRenderer.enabled = true;
        }
    }

    private void OnWhiteEventTriggered()
    {
        Debug.Log("����������ɣ�");
    }

    private void OnDestroy()
    {
        EventCenter.Instance.Unsubscribe("white", OnWhiteEventTriggered);
    }
}