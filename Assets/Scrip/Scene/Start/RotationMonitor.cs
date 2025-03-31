using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flockaroo
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class RotationMonitor : MonoBehaviour
    {
        [Header("目标物体")]
        public GameObject targetObject; // 需要检测旋转的物体

        [Header("效果物体")]
        public GameObject effectObject; // 带有ColoredPencilsEffect脚本的物体

        private ColoredPencilsEffect pencilEffect;

        void Start()
        {
            // 自动获取名为"Object"的物体
            if (effectObject == null)
                effectObject = GameObject.Find("Object");

            if (effectObject != null)
                pencilEffect = effectObject.GetComponent<ColoredPencilsEffect>();

            if (pencilEffect == null)
                Debug.LogError("未找到ColoredPencilsEffect脚本");
        }

        void Update()
        {
            if (targetObject == null || pencilEffect == null) return;

            // 获取标准化后的Y轴旋转角度
            float rotationY = NormalizeAngle(targetObject.transform.eulerAngles.y);

            // 计算轮廓值
            float outlineValue = CalculateOutline(rotationY);

            // 应用轮廓值
            pencilEffect.outlines = outlineValue;
        }

        float NormalizeAngle(float angle)
        {
            // 将角度标准化到-180~180范围
            angle %= 360;
            if (angle > 180) angle -= 360;
            return angle;
        }

        float CalculateOutline(float angle)
        {
            // 处理关键角度之间的线性插值
            float absAngle = Mathf.Abs(angle);

            // 处理0°到180°范围
            if (angle >= 0 && angle <= 180)
            {
                if (angle <= 45)
                    return Mathf.Lerp(1f, 0f, angle / 45f);
                else if (angle <= 90)
                    return Mathf.Lerp(0f, 1f, (angle - 45) / 45f);
                else if (angle <= 135)
                    return Mathf.Lerp(1f, 0f, (angle - 90) / 45f);
                else
                    return Mathf.Lerp(0f, 1f, (angle - 135) / 45f);
            }
            // 处理-180°到0°范围
            else
            {
                if (angle >= -45)
                    return Mathf.Lerp(1f, 0f, -angle / 45f);
                else if (angle >= -90)
                    return Mathf.Lerp(0f, 1f, (-angle - 45) / 45f);
                else if (angle >= -135)
                    return Mathf.Lerp(1f, 0f, (-angle - 90) / 45f);
                else
                    return Mathf.Lerp(0f, 1f, (-angle - 135) / 45f);
            }
        }
    }
}