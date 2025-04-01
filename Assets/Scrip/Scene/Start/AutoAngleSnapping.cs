using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAngleSnapping: MonoBehaviour
{
    [Header("��ת����")]
    [Tooltip("�Ƕȼ����ֵ")]
    public float snapThreshold = 5f;
    [Tooltip("�������ʱ��")]
    public float snapDuration = 0.5f;

    private float currentRotationY;
    private float targetRotationY;
    private bool isSnapping;
    private float snapStartTime;

    void Update()
    {
        currentRotationY = NormalizeAngle(transform.eulerAngles.y);

        // ����Ƿ���Ҫ����
        if (!isSnapping && ShouldSnap(currentRotationY, out targetRotationY))
        {
            StartSnapping();
        }

        // ִ�ж������
        if (isSnapping)
        {
            SmoothRotate();
        }
    }

    bool ShouldSnap(float angle, out float target)
    {
        float distanceTo0 = Mathf.Abs(angle);
        float distanceTo180 = Mathf.Abs(angle - 180f);

        // ȡ����Ļ�׼�Ƕ�
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
        // �����ֵ����
        float t = (Time.time - snapStartTime) / snapDuration;
        float newAngle = Mathf.LerpAngle(currentRotationY, targetRotationY, t);

        // Ӧ����ת
        transform.rotation = Quaternion.Euler(0, newAngle, 0);

        // ��ɼ��
        if (t >= 1f)
        {
            isSnapping = false;
            transform.rotation = Quaternion.Euler(0, targetRotationY, 0); // ȷ����ȷ����
        }
    }

    // ��׼���Ƕȵ�-180~180��Χ��������ҳ1�ĽǶȴ�����[1](@ref)��
    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}