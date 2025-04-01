using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAngleSnapping: MonoBehaviour
{
    [Header("旋转参数")]
    [Tooltip("角度检测阈值")]
    public float snapThreshold = 5f;
    [Tooltip("对齐持续时间")]
    public float snapDuration = 0.5f;

    private float currentRotationY;
    private float targetRotationY;
    private bool isSnapping;
    private float snapStartTime;

    void Update()
    {
        currentRotationY = NormalizeAngle(transform.eulerAngles.y);

        // 检测是否需要对齐
        if (!isSnapping && ShouldSnap(currentRotationY, out targetRotationY))
        {
            StartSnapping();
        }

        // 执行对齐过程
        if (isSnapping)
        {
            SmoothRotate();
        }
    }

    bool ShouldSnap(float angle, out float target)
    {
        float distanceTo0 = Mathf.Abs(angle);
        float distanceTo180 = Mathf.Abs(angle - 180f);

        // 取最近的基准角度
        if (distanceTo0 <= snapThreshold || distanceTo180 <= snapThreshold)
        {
            target = (distanceTo0 < distanceTo180) ? 0f : 180f;
            return true;
        }
        target = 0f;
        return false;
    }

    void StartSnapping()
    {
        isSnapping = true;
        snapStartTime = Time.time;
    }

    void SmoothRotate()
    {
        // 计算插值进度
        float t = (Time.time - snapStartTime) / snapDuration;
        float newAngle = Mathf.LerpAngle(currentRotationY, targetRotationY, t);

        // 应用旋转
        transform.rotation = Quaternion.Euler(0, newAngle, 0);

        // 完成检查
        if (t >= 1f)
        {
            isSnapping = false;
            transform.rotation = Quaternion.Euler(0, targetRotationY, 0); // 确保精确对齐
        }
    }

    // 标准化角度到-180~180范围（基于网页1的角度处理方案[1](@ref)）
    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}