using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

namespace Flockaroo
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    //[AddComponentMenu("Image Effects/Artistic/ColoredPencils")]
    public class RotationMonitor : MonoBehaviour
    {
        [Header("目标物体")]
        public GameObject targetObject; // 需要检测旋转的物体

        [Header("效果物体")]
        public GameObject effectObject; // 带有ColoredPencilsEffect脚本的物体

        private ColoredPencilsEffect pencilEffect;

        void Start()
        {
            // 自动获取名为"Object"的物体[7](@ref)
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

            // 获取标准化后的Y轴旋转角度[8](@ref)
            float rotationY = NormalizeAngle(targetObject.transform.eulerAngles.y);

            // 计算轮廓值
            float outlineValue = CalculateOutline(rotationY);

            // 应用轮廓值[1](@ref)
            pencilEffect.outlines = outlineValue;
        }

        float NormalizeAngle(float angle)
        {
            // 将角度标准化到-180~180范围[8](@ref)
            angle %= 360;
            if (angle > 180) angle -= 360;
            return angle;
        }

        float CalculateOutline(float angle)
        {
            // 处理四个关键方向间的线性插值[1](@ref)
            float absAngle = Mathf.Abs(angle);

            if (absAngle <= 90)
                return Mathf.Lerp(1, 0, absAngle / 90);
            else
                return Mathf.Lerp(0, 1, (absAngle - 90) / 90);
        }
    }
}