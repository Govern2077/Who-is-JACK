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
        [Header("Ŀ������")]
        public GameObject targetObject; // ��Ҫ�����ת������

        [Header("Ч������")]
        public GameObject effectObject; // ����ColoredPencilsEffect�ű�������

        private ColoredPencilsEffect pencilEffect;

        void Start()
        {
            // �Զ���ȡ��Ϊ"Object"������[7](@ref)
            if (effectObject == null)
                effectObject = GameObject.Find("Object");

            if (effectObject != null)
                pencilEffect = effectObject.GetComponent<ColoredPencilsEffect>();

            if (pencilEffect == null)
                Debug.LogError("δ�ҵ�ColoredPencilsEffect�ű�");
        }

        void Update()
        {
            if (targetObject == null || pencilEffect == null) return;

            // ��ȡ��׼�����Y����ת�Ƕ�[8](@ref)
            float rotationY = NormalizeAngle(targetObject.transform.eulerAngles.y);

            // ��������ֵ
            float outlineValue = CalculateOutline(rotationY);

            // Ӧ������ֵ[1](@ref)
            pencilEffect.outlines = outlineValue;
        }

        float NormalizeAngle(float angle)
        {
            // ���Ƕȱ�׼����-180~180��Χ[8](@ref)
            angle %= 360;
            if (angle > 180) angle -= 360;
            return angle;
        }

        float CalculateOutline(float angle)
        {
            // �����ĸ��ؼ����������Բ�ֵ[1](@ref)
            float absAngle = Mathf.Abs(angle);

            if (absAngle <= 90)
                return Mathf.Lerp(1, 0, absAngle / 90);
            else
                return Mathf.Lerp(0, 1, (absAngle - 90) / 90);
        }
    }
}