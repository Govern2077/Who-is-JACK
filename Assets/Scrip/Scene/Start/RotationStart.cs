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
    [SerializeField] private float logic1MaxRotation = 540f;
    [SerializeField] private float logic2MaxRotation = 720f;
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

    [Header("�Զ���������")]
    [Tooltip("�����Ƕ���ֵ(��)")] public float snapThreshold = 5f;
    [Tooltip("��������ʱ��(��)")] public float snapDuration = 0.5f;

    private Vector3 lastMousePosition;
    private bool isMousePressed = false;
    private float rotationVelocityY = 0f;
    private float currentVelocityY = 0f;
    private float[] snapAngles = { 0f, 90f, 180f, 270f, 360f };
    private float snapTargetAngle = -1f;
    private float snapStartAngle;
    private float snapStartTime;

    void Update()
    {
        HandleDualLogicRotation();
        UpdateMeshRenderers();
        HandleAutoSnap();
    }

    private void HandleDualLogicRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            lastMousePosition = Input.mousePosition;
            snapTargetAngle = -1f; // �϶�ʱ�ж�����
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

            float maxRotation = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
            accumulatedRotationY = Mathf.Clamp(
                accumulatedRotationY + rotateY,
                0,
                maxRotation
            );

            ApplySmartRotation();
        }

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

    private void ApplySmartRotation()
    {
        targetObject.transform.rotation = Quaternion.Euler(
            targetObject.transform.eulerAngles.x,
            accumulatedRotationY,
            targetObject.transform.eulerAngles.z
        );
    }

    private void UpdateMeshRenderers()
    {
        float currentY = accumulatedRotationY;
        int index = Mathf.FloorToInt(currentY / 90f);

        GameObject[] activeList = useLogic1 ? logic1Objects : logic2Objects;
        GameObject[] inactiveList = useLogic1 ? logic2Objects : logic1Objects;

        SetListVisibility(inactiveList, false);

        if (activeList.Length > 0)
        {
            int clampedIndex = Mathf.Clamp(index, 0, activeList.Length - 1);
            SetSingleActive(activeList, clampedIndex);
        }
    }

    private void HandleAutoSnap()
    {
        if (isMousePressed) return;

        if (snapTargetAngle < 0)
        {
            FindClosestSnapAngle();
        }
        else
        {
            PerformSnapAnimation();
        }
    }

    private void FindClosestSnapAngle()
    {
        float minDistance = float.MaxValue;
        float closestAngle = -1f;

        foreach (float angle in snapAngles)
        {
            float distance = Mathf.Abs(accumulatedRotationY - angle);
            if (distance <= snapThreshold && distance < minDistance)
            {
                minDistance = distance;
                closestAngle = angle;
            }
        }

        if (closestAngle >= 0)
        {
            snapTargetAngle = closestAngle;
            snapStartAngle = accumulatedRotationY;
            snapStartTime = Time.time;
            rotationVelocityY = 0f;
        }
    }

    private void PerformSnapAnimation()
    {
        float elapsed = Time.time - snapStartTime;
        float t = Mathf.Clamp01(elapsed / snapDuration);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        accumulatedRotationY = Mathf.Lerp(snapStartAngle, snapTargetAngle, smoothT);

        float maxRotation = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
        accumulatedRotationY = Mathf.Clamp(accumulatedRotationY, 0f, maxRotation);

        ApplySmartRotation();

        if (t >= 1f)
        {
            accumulatedRotationY = snapTargetAngle;
            snapTargetAngle = -1f;
            ApplySmartRotation();
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

    #region ���Թ���
    void OnDrawGizmosSelected()
    {
        if (showRotationGizmo && targetObject != null)
        {
            float currentMax = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
            Color gizmoColor = useLogic1 ? logic1Color : logic2Color;

            Gizmos.color = gizmoColor;
            float radius = currentMax / 100f;
            Gizmos.DrawWireSphere(targetObject.transform.position, radius);

            UnityEditor.Handles.Label(
                targetObject.transform.position + Vector3.up * 2,
                $"��ǰ�Ƕ�: {accumulatedRotationY:F1}�� / {currentMax}��\n" +
                $"��ǰģʽ: {(useLogic1 ? "�߼�1" : "�߼�2")}"
            );

            // �����Ƕȿ��ӻ�
            Gizmos.color = Color.green;
            foreach (float angle in snapAngles)
            {
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Gizmos.DrawLine(targetObject.transform.position,
                    targetObject.transform.position + dir * 2f);
            }
        }
    }

    public void DebugRotationValue()
    {
        Debug.Log($"��ǰ�ۻ��Ƕ�: {accumulatedRotationY}��");
    }
    #endregion
}