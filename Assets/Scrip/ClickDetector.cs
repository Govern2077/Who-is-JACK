using Febucci.UI.Examples;
using UnityEngine;
using System.Collections;

public class ClickDetector : MonoBehaviour
{
    [Header("�����ĶԻ�������")]
    [SerializeField]
    public ExampleEvents exampleEvents; // ��ק�� ExampleEvents ����

    [Header("���ð������ĳ���ʱ��")]
    [SerializeField]
    private float disableDuration = 1f; // Ĭ�Ͻ���2��

    // ��������������Colliderʱ����
    void OnMouseDown()
    {
        Debug.Log($"��������壺{gameObject.name}");

        // ���öԻ�����
        if (exampleEvents != null)
        {
            exampleEvents.canContinue = false;
            StartCoroutine(EnableContinueAfterDelay());
        }
    }

    // Э�̣���ָ��ʱ���ָ���������Ի�
    IEnumerator EnableContinueAfterDelay()
    {
        yield return new WaitForSeconds(disableDuration);
        exampleEvents.canContinue = true;
        Debug.Log("��������������Ի�");
    }
}