using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedRotationController : MonoBehaviour
{
    [Header("核心参数")]
    public GameObject targetObject;
    public bool useLogic1 = true;
    [Range(0.01f, 1f)] public float rotateSpeed = 0.05f;
    [Tooltip("惯性减速时间(秒)")] public float decelerationTime = 0.5f;

    [Header("旋转限制")]
    [SerializeField] private float logic1MaxRotation = 540f;  // 修改为540度[3](@ref)
    [SerializeField] private float logic2MaxRotation = 720f;  // 新增逻辑2限制[3](@ref)
    [Space(10)]

    [Header("物体列表")]
    public GameObject[] logic1Objects;
    public GameObject[] logic2Objects;

    [Header("累积旋转量")]
    [SerializeField] private float accumulatedRotationY = 0f;

    [Header("调试工具")]
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

    // 双逻辑旋转处理（基于网页3的Clamp方案）
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

            // 动态角度限制[3,4](@ref)
            float maxRotation = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
            accumulatedRotationY = Mathf.Clamp(
                accumulatedRotationY + rotateY,
                0,
                maxRotation
            );

            ApplySmartRotation();
        }

        // 惯性处理（优化网页6的减速方案）
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

    // 智能旋转应用（基于网页4的旋转控制）
    private void ApplySmartRotation()
    {
        targetObject.transform.rotation = Quaternion.Euler(
            targetObject.transform.eulerAngles.x,
            accumulatedRotationY,
            targetObject.transform.eulerAngles.z
        );
    }

    // 物体显示逻辑（保持网页1的区间映射）
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

    #region 辅助方法
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

    #region 调试工具（增强网页3的可视化）
    void OnDrawGizmosSelected()
    {
        if (showRotationGizmo && targetObject != null)
        {
            float currentMax = useLogic1 ? logic1MaxRotation : logic2MaxRotation;
            Color gizmoColor = useLogic1 ? logic1Color : logic2Color;

            // 绘制角度范围指示器
            Gizmos.color = gizmoColor;
            float radius = currentMax / 100f;
            Gizmos.DrawWireSphere(targetObject.transform.position, radius);

            // 添加角度文本标签
            UnityEditor.Handles.Label(
                targetObject.transform.position + Vector3.up * 2,
                $"当前角度: {accumulatedRotationY:F1}° / {currentMax}°\n" +
                $"当前模式: {(useLogic1 ? "逻辑1" : "逻辑2")}"
            );
        }
    }

    public void DebugRotationValue()
    {
        Debug.Log($"当前累积角度: {accumulatedRotationY}°");
    }
    #endregion
}