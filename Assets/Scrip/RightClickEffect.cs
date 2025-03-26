using Flockaroo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickEffect : MonoBehaviour
{
    private ColoredPencilsEffect coloredPencilsEffect;  // ���� ColoredPencilsEffect �ű�
    private List<Outline> outlineObjects;  // �洢��������� Outline �ű�
    public bool isChanging = false;  // �ж��Ƿ����ڽ�����ֵ����
    private float transitionTime = 0.5f;  // ��ֵ�仯�Ĺ���ʱ��
    private float currentTime = 0f;  // ��ǰ���ɵ�ʱ��

    // �ֶ��϶����� A �е� RotateObjectWithMouse �ű����˱���
    public RotateObjectWithMouse rotateObjectWithMouse;  // ���� RotateObjectWithMouse �ű�

    // public ���ڼ�� outline.OutlineWidth �Ƿ�Ϊ 0
    public bool white = false;

    // ���ڴ洢�����Ҫƽ������͸���ȵ�����Ĳ���
    public List<Renderer> transparentObjectRenderers;  // ��Ҫƽ������͸���ȵ������б�

    // ���� MoveObjectOnEsc �ű�
    public MoveObjectOnEsc moveObjectOnEsc;  // ���� MoveObjectOnEsc �ű�

    private void Start()
    {
        // ��ȡ Main Camera �ϵ� ColoredPencilsEffect �ű�
        coloredPencilsEffect = Camera.main.GetComponent<ColoredPencilsEffect>();

        // ��ȡ��������� Outline �ű�
        outlineObjects = new List<Outline>(FindObjectsOfType<Outline>());

        // �����¼���ʹ��������������Э��
        EventCenter.Instance.Subscribe("white", () => StartCoroutine(ChangeValues()));
    }

    private void Update()
    {
        // ��� moveObjectOnEsc �ű��е� look �Ƿ�Ϊ false����ִ���Ҽ�����¼�
        if (moveObjectOnEsc != null && !moveObjectOnEsc.look 
            && rotateObjectWithMouse != null 
            && rotateObjectWithMouse.change 
            && Input.GetMouseButtonDown(1) 
            && !isChanging)
        {
            // ���� white �¼�
            EventCenter.Instance.TriggerEvent("white");
        }
    }

    private IEnumerator ChangeValues()
    {
        if (isChanging)
            yield break;
        isChanging = true;  // �������ڽ��й���

        // ��ȡ��ǰ�� outlines �� hatches ֵ
        float startOutline = coloredPencilsEffect.outlines;
        float startHatch = coloredPencilsEffect.hatches;

        // ��ȡ��������ĵ�ǰ outlineWidth ��Ŀ��ֵ
        float startOutlineWidth = outlineObjects[0].OutlineWidth;

        // ����Ŀ��ֵ�ĳ�ʼֵ
        float targetOutline = startOutline;
        float targetHatch = startHatch;
        float targetOutlineWidth = startOutlineWidth;

        // ���ݵ�ǰֵ�����任��Ŀ��ֵ
        if (startOutlineWidth == 2f)
        {
            targetOutlineWidth = 0f;
        }
        else if (startOutlineWidth == 0f)
        {
            targetOutlineWidth = 2f;
        }

        if (startHatch == 1f)
        {
            targetHatch = 0.5f;
        }
        else if (startHatch == 0.5f)
        {
            targetHatch = 1f;
        }

        if (startOutline == 1f)
        {
            targetOutline = 0f;
        }
        else if (startOutline == 0f)
        {
            targetOutline = 1f;
        }

        // �洢ÿ������ĵ�ǰ͸����ֵ
        List<float> startAlphas = new List<float>();
        foreach (Renderer transparentObject in transparentObjectRenderers)
        {
            Color startColor = transparentObject.material.GetColor("_Color");
            startAlphas.Add(startColor.a);
        }

        // ����Ŀ��͸����
        List<float> targetAlphas = new List<float>();
        foreach (float startAlpha in startAlphas)
        {
            targetAlphas.Add(startAlpha == 0f ? 1f : 0f);  // 0 -> 1, 1 -> 0
        }

        // ƽ������
        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;  // ��ȷ��ȡʱ������
            float t = currentTime / transitionTime;

            // �ı� ColoredPencilsEffect �е� outlines �� hatches
            coloredPencilsEffect.outlines = Mathf.Lerp(startOutline, targetOutline, t);
            coloredPencilsEffect.hatches = Mathf.Lerp(startHatch, targetHatch, t);

            // �ı���������� outlineWidth
            foreach (Outline outline in outlineObjects)
            {
                outline.OutlineWidth = Mathf.Lerp(startOutlineWidth, targetOutlineWidth, t);
            }

            // �ı�ÿ��͸�������͸����
            for (int i = 0; i < transparentObjectRenderers.Count; i++)
            {
                Renderer transparentObject = transparentObjectRenderers[i];
                float startAlpha = startAlphas[i];
                float targetAlpha = targetAlphas[i];

                // ƽ��͸���ȱ仯
                Color newColor = new Color(transparentObject.material.color.r, transparentObject.material.color.g, transparentObject.material.color.b, Mathf.Lerp(startAlpha, targetAlpha, t));
                transparentObject.material.SetColor("_Color", newColor);
            }

            yield return null;
        }

        // ����ֵ����ΪĿ��ֵ
        coloredPencilsEffect.outlines = targetOutline;
        coloredPencilsEffect.hatches = targetHatch;

        // ����������������� outlineWidth
        foreach (Outline outline in outlineObjects)
        {
            outline.OutlineWidth = targetOutlineWidth;
        }

        // ����ÿ��͸�����������͸����
        for (int i = 0; i < transparentObjectRenderers.Count; i++)
        {
            Renderer transparentObject = transparentObjectRenderers[i];
            float targetAlpha = targetAlphas[i];
            Color finalColor = new Color(transparentObject.material.color.r, transparentObject.material.color.g, transparentObject.material.color.b, targetAlpha);
            transparentObject.material.SetColor("_Color", finalColor);
        }

        // ���� white ����ֵ����� outline.OutlineWidth �Ƿ�Ϊ 0
        white = (outlineObjects[0].OutlineWidth != 0f);

        // ���ù���ʱ�䣬׼����һ�β���
        currentTime = 0f;
        isChanging = false;  // �����ٴν��в���
    }

    private void OnDestroy()
    {
        // ȷ��������ʱȡ������
        EventCenter.Instance.Unsubscribe("white", () => StartCoroutine(ChangeValues()));
    }
}
