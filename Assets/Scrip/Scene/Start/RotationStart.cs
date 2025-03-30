using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedRotationController : MonoBehaviour
{
    [Header("���Ĳ���")]
    public GameObject targetObject;
    public bool useLogic1 = true;
    [Range(0.01f, 1f)] public float rotateSpeed = 0.05f;
    [Tooltip("���Լ���ʱ��(��)")] public float decelerationTime = 0.5f;

    [Header("��ת����")]
    [SerializeField] private float logic1MaxRotation = 540f;  // �޸�Ϊ540��[3](@ref)
    [SerializeField] private float logic2MaxRotation = 720f;  // �����߼�2����[3](@ref)
    [Space(10)]

    [Header("�����б�")]
    public GameObject[] logic1Objects;
    public GameObject[] logic2Objects;

    [Header("�ۻ���ת��")]
    [SerializeField] private float accumulatedRotationY = 0f;

    [Header("���Թ���")]
    public bool showRotationGizmo = true;
    public Color logic1Color = Color.yellow;
    public Color logic2Color = Color.cyan;

    private Vector3 lastMousePosition;
    private bool isMousePressed = false;
    private float rotationVelocityY = 0f;
    private float currentVelocityY = 0f;

    void Update()
    {
        HandleDualLogicRotation();
        UpdateMeshRenderers();
    }

    // ˫�߼���ת����������ҳ3��Clamp������
    private void HandleDualLogicRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            rotationVelocityY = currentVelocityY;
        }

        if (isMousePressed && targetObject != null)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            float rotateY = -mouseDelta.x * rotateSpeed;
            currentVelocityY = rotateY;

            // ��̬�Ƕ�����[3,4](@ref)
            float maxRotation = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
            accumulatedRotationY = Mathf.Clamp(
                accumulatedRotationY + rotateY,
                0,
                maxRotation
            );

            ApplySmartRotation();
        }

        // ���Դ����Ż���ҳ6�ļ��ٷ�����
        if (!isMousePressed && Mathf.Abs(rotationVelocityY) > 0.01f)
        {
            float decelerationFactor = Time.deltaTime / decelerationTime;
            rotationVelocityY = Mathf.Lerp(rotationVelocityY, 0, decelerationFactor);

            float maxRotation = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
            accumulatedRotationY = Mathf.Clamp(
                accumulatedRotationY + rotationVelocityY,
                0,
                maxRotation
            );

            ApplySmartRotation();
        }
    }

    // ������תӦ�ã�������ҳ4����ת���ƣ�
    private void ApplySmartRotation()
    {
        targetObject.transform.rotation = Quaternion.Euler(
            targetObject.transform.eulerAngles.x,
            accumulatedRotationY,
            targetObject.transform.eulerAngles.z
        );
    }

    // ������ʾ�߼���������ҳ1������ӳ�䣩
    private void UpdateMeshRenderers()
    {
        float currentY = accumulatedRotationY;
        int index = Mathf.FloorToInt((currentY + 90) / 180);

        GameObject[] activeList = useLogic1 ? logic1Objects : logic2Objects;
        GameObject[] inactiveList = useLogic1 ? logic2Objects : logic1Objects;

        SetListVisibility(inactiveList, false);

        if (activeList.Length > 0)
        {
            int clampedIndex = Mathf.Clamp(
                (index % activeList.Length + activeList.Length) % activeList.Length,
                0,
                activeList.Length - 1
            );
            SetSingleActive(activeList, clampedIndex);
        }
    }

    #region ��������
    private void SetSingleActive(GameObject[] list, int index)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] != null)
            {
                bool shouldEnable = (i == index);
                var renderer = list[i].GetComponent<MeshRenderer>();
                if (renderer != null) renderer.enabled = shouldEnable;
            }
        }
    }

    private void SetListVisibility(GameObject[] list, bool state)
    {
        foreach (var obj in list)
        {
            if (obj != null)
            {
                var renderer = obj.GetComponent<MeshRenderer>();
                if (renderer != null) renderer.enabled = state;
            }
        }
    }
    #endregion

    #region ���Թ��ߣ���ǿ��ҳ3�Ŀ��ӻ���
    void OnDrawGizmosSelected()
    {
        if (showRotationGizmo && targetObject != null)
        {
            float currentMax = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
            Color gizmoColor = useLogic1 ? logic1Color : logic2Color;

            // ���ƽǶȷ�Χָʾ��
            Gizmos.color = gizmoColor;
            float radius = currentMax / 100f;
            Gizmos.DrawWireSphere(targetObject.transform.position, radius);

            // ��ӽǶ��ı���ǩ
            UnityEditor.Handles.Label(
                targetObject.transform.position + Vector3.up * 2,
                $"��ǰ�Ƕ�: {accumulatedRotationY:F1}�� / {currentMax}��\n" +
                $"��ǰģʽ: {(useLogic1 ? "�߼�1" : "�߼�2")}"
            );
        }
    }

    public void DebugRotationValue()
    {
        Debug.Log($"��ǰ�ۻ��Ƕ�: {accumulatedRotationY}��");
    }
    #endregion
}