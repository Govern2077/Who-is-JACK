using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flockaroo
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class RotationMonitor : MonoBehaviour
    {
        [Header("Ŀ������")]
        public GameObject targetObject; // ��Ҫ�����ת������

        [Header("Ч������")]
        public GameObject effectObject; // ����ColoredPencilsEffect�ű�������

        private ColoredPencilsEffect pencilEffect;

        void Start()
        {
            // �Զ���ȡ��Ϊ"Object"������
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

            // ��ȡ��׼�����Y����ת�Ƕ�
            float rotationY = NormalizeAngle(targetObject.transform.eulerAngles.y);

            // ��������ֵ
            float outlineValue = CalculateOutline(rotationY);

            // Ӧ������ֵ
            pencilEffect.outlines = outlineValue;
        }

        float NormalizeAngle(float angle)
        {
            // ���Ƕȱ�׼����-180~180��Χ
            angle %= 360;
            if (angle > 180) angle -= 360;
            return angle;
        }

        float CalculateOutline(float angle)
        {
            // ����ؼ��Ƕ�֮������Բ�ֵ
            float absAngle = Mathf.Abs(angle);

            // ����0�㵽180�㷶Χ
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
            // ����-180�㵽0�㷶Χ
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