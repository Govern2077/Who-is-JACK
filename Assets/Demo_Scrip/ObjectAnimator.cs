using System;
using System.Collections;
using UnityEngine;

public class ObjectAnimator : MonoBehaviour
{
    public Transform objectA; // ����A��Transform
    private bool isAnimating = false; // ����Ƿ����ڽ��ж���
    public bool  hasClicked = false;  // ����Ƿ��Ѿ������

    void Start()
    {
        // ͨ��EventCenter����"change"�¼�
        EventCenter.Instance.Subscribe("change", OnChangeEventTriggered);
    }

    void Update()
    {
        // ֻ�ڵ�һ�ε��ʱ��������
        if (!hasClicked && Input.GetMouseButtonDown(0)) // 0 ��������
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ��������������A���򴥷�change�¼�
                if (hit.transform == objectA)
                {
                    Debug.Log("Triggering 'change' event.");
                    EventCenter.Instance.TriggerEvent("change");

                    // ����Ѿ��������ȷ���Ժ󲻻��ٴ���
                    hasClicked = true;
                }
            }
        }

        // ʾ����������ڶ�������������������ת��λ��
        if (!isAnimating)
        {
            // �����ڴ˴�������������������������ת��λ��
            // ���磺������ת
            //objectA.Rotate(Vector3.up * 20f * TimeTime); // ÿ����ת20��
            // �����λ��
            // objectA.position += Vector3.forward * TimeTime; // ÿ����ǰ�ƶ�
        }
    }

    // ��change�¼�����ʱ�������������
    private void OnChangeEventTriggered()
    {
        // ��ʼЭ��ִ��ƽ�����ɶ���
        if (!isAnimating) // ȷ�����ظ���������
        {
            isAnimating = true;
            StartCoroutine(AnimateObject());
        }
    }

    // Э�̣���0.5���ڽ�����A��scale, rotation, positionƽ�����ɵ�Ŀ��ֵ
    private IEnumerator AnimateObject()
    {
        Vector3 initialScale = objectA.localScale;
        Quaternion initialRotation = objectA.rotation;
        Vector3 initialPosition = objectA.position;

        Vector3 targetScale = new Vector3(2f, 2f, 2f);
        Quaternion targetRotation = Quaternion.Euler(30f, 30f, 30f);
        Vector3 targetPosition = new Vector3(2f, 2f, -2f);

        float duration = 0.5f;
        float t = 0;

        // ƽ������
        while (t < duration)
        {
            t += Time.deltaTime;
            float pct = t / duration;

            // ʹ��Lerp��ƽ������
            objectA.localScale = Vector3.Lerp(initialScale, targetScale, pct);
            objectA.rotation = Quaternion.Lerp(initialRotation, targetRotation, pct);
            objectA.position = Vector3.Lerp(initialPosition, targetPosition, pct);

            yield return null;
        }

        // ȷ�����մﵽĿ��ֵ
        objectA.localScale = targetScale;
        objectA.rotation = targetRotation;
        objectA.position = targetPosition;

        // ���������󣬿��Կ����������ת��λ��
        isAnimating = false; // ������ɣ����������������
    }
}
