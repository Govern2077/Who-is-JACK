using Flockaroo;
using System.Collections.Generic;
using UnityEngine;

public class RightClickEffect : MonoBehaviour
{
    private ColoredPencilsEffect coloredPencilsEffect;  // ���� ColoredPencilsEffect �ű�
    private List<Outline> outlineObjects;  // �洢��������� Outline �ű�
    private bool isChanging = false;  // �ж��Ƿ����ڽ�����ֵ����
    private bool isReversed = false;  // �ж��Ƿ��Ѿ���ת��ֵ
    private float transitionTime = 0.5f;  // ��ֵ�仯�Ĺ���ʱ��
    private float currentTime = 0f;  // ��ǰ���ɵ�ʱ��

    private void Start()
    {
        // ��ȡ Main Camera �ϵ� ColoredPencilsEffect �ű�
        coloredPencilsEffect = Camera.main.GetComponent<ColoredPencilsEffect>();

        // ��ȡ��������� Outline �ű�
        outlineObjects = new List<Outline>(FindObjectsOfType<Outline>());
    }

    private void Update()
    {
        // �������Ҽ�����
        if (Input.GetMouseButtonDown(1) && !isChanging)
        {
            // ��ֹ�ڹ��ɹ������ظ�����
            StartCoroutine(ChangeValues());
        }
    }

    private System.Collections.IEnumerator ChangeValues()
    {
        isChanging = true;  // �������ڽ��й���

        // �ж��Ƿ��ǵ�һ�α仯
        if (!isReversed)
        {
            // ��һ�ΰ��£���ʼ����
            float startOutline = coloredPencilsEffect.outlines;
            float startHatch = coloredPencilsEffect.hatches;

            // Ŀ��ֵ�ֱ��� 0 �� 0.5
            float targetOutline = 0f;
            float targetHatch = 0.5f;

            // ��ȡ��������ĵ�ǰ outlineWidth ��Ŀ��ֵ
            float startOutlineWidth = outlineObjects[0].OutlineWidth;
            float targetOutlineWidth = 10f;

            // ƽ������
            while (currentTime < transitionTime)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / transitionTime;

                // �ı� ColoredPencilsEffect �е� outlines �� hatches
                coloredPencilsEffect.outlines = Mathf.Lerp(startOutline, targetOutline, t);
                coloredPencilsEffect.hatches = Mathf.Lerp(startHatch, targetHatch, t);

                // �ı���������� outlineWidth
                foreach (Outline outline in outlineObjects)
                {
                    outline.OutlineWidth = Mathf.Lerp(startOutlineWidth, targetOutlineWidth, t);
                }

                yield return null;
            }

            // ����ֵ����Ϊ 0 �� 0.5
            coloredPencilsEffect.outlines = targetOutline;
            coloredPencilsEffect.hatches = targetHatch;

            // ����������������� outlineWidth
            foreach (Outline outline in outlineObjects)
            {
                outline.OutlineWidth = targetOutlineWidth;
            }

            // �����ѷ�ת��ǣ���һ�ΰ��½���ָ�
            isReversed = true;
        }
        else
        {
            // �ڶ��ΰ��£��ָ�ԭֵ
            float startOutline = coloredPencilsEffect.outlines;
            float startHatch = coloredPencilsEffect.hatches;

            // Ŀ��ֵ�ֱ��� 1 �� 1
            float targetOutline = 1f;
            float targetHatch = 1f;

            // ��ȡ��������ĵ�ǰ outlineWidth ��Ŀ��ֵ
            float startOutlineWidth = outlineObjects[0].OutlineWidth;
            float targetOutlineWidth = 0f;

            // ƽ������
            while (currentTime < transitionTime)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / transitionTime;

                // �ı� ColoredPencilsEffect �е� outlines �� hatches
                coloredPencilsEffect.outlines = Mathf.Lerp(startOutline, targetOutline, t);
                coloredPencilsEffect.hatches = Mathf.Lerp(startHatch, targetHatch, t);

                // �ı���������� outlineWidth
                foreach (Outline outline in outlineObjects)
                {
                    outline.OutlineWidth = Mathf.Lerp(startOutlineWidth, targetOutlineWidth, t);
                }

                yield return null;
            }

            // ����ֵ����Ϊ 1 �� 1
            coloredPencilsEffect.outlines = targetOutline;
            coloredPencilsEffect.hatches = targetHatch;

            // ����������������� outlineWidth
            foreach (Outline outline in outlineObjects)
            {
                outline.OutlineWidth = targetOutlineWidth;
            }

            // ���ñ�ǣ�׼����һ�η�ת
            isReversed = false;
        }

        // ���ù���ʱ�䣬׼����һ�β���
        currentTime = 0f;
        isChanging = false;  // �����ٴν��в���
    }
}
