using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))] // ȷ��������ײ��
public class DetectiveBoardTrigger : MonoBehaviour
{
    [System.Serializable]
    public class PointPair
    {
        public int index;
        public List<GameObject> targetObjects; // ����Ϊ�������ı�����
    }

    [Header("��������")]
    [Tooltip("��̽�涯����������")]
    public List<PointPair> positionPairs = new List<PointPair>();

    [Header("��������")]
    [SerializeField] string triggerTag = "Player"; // ֻ��Ӧ�ض���ǩ������
    [SerializeField] int targetIndex = 0;          // Ҫ������PointPair����

    [Header("��������")]
    [SerializeField] float interval = 3f;         // �����ü��ʱ��

    [Header("����ģʽ")]
    [SerializeField] bool showLogs = true;         // ��־����

    private bool isPlaying = false;                // ��ֹ�ظ�����

    // ��ʼ��ʱ��֤���
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"���� {name} ����ײ��δ����Ϊ�����������Զ�����");
            col.isTrigger = true;
        }
    }

    // ��ײ�������
    void OnTriggerEnter(Collider other)
    {
        if (!isPlaying && other.CompareTag(triggerTag))
        {
            StartAnimationSequence();
        }
    }

    public void StartAnimationSequence()
    {
        if (isPlaying)
        {
            if (showLogs) Debug.Log("�������ڲ����У�����������");
            return;
        }

        PointPair targetPair = positionPairs.Find(p => p.index == targetIndex);
        if (targetPair == null)
        {
            Debug.LogError($"δ�ҵ�����Ϊ {targetIndex} ��PointPair����");
            return;
        }

        StartCoroutine(ProcessAnimationSequence(targetPair.targetObjects));
        isPlaying = true;
    }

    IEnumerator ProcessAnimationSequence(List<GameObject> targets)
    {
        if (targets.Count == 0)
        {
            if (showLogs) Debug.LogWarning("Ŀ�������б�Ϊ��");
            yield break;
        }

        Animator firstAnimator = null;
        GameObject firstValidObject = null;

        // ��һ�׶Σ�Ѱ���׸���Ч����
        foreach (GameObject obj in targets)
        {
            if (obj == null) continue;

            Animator anim = obj.GetComponent<Animator>();
            if (anim != null && HasBoolParameter("action", anim))
            {
                firstAnimator = anim;
                firstValidObject = obj;
                break;
            }
        }

        if (firstAnimator == null)
        {
            Debug.LogWarning("δ�ҵ���Ч��ʼ����");
            yield break;
        }

        // �ڶ��׶Σ�˳�򼤻��
        foreach (GameObject obj in targets)
        {
            if (obj == null)
            {
                if (showLogs) Debug.LogWarning("��⵽���������ã�����");
                continue;
            }

            Animator currentAnim = obj.GetComponent<Animator>();
            if (currentAnim == null)
            {
                if (showLogs) Debug.LogWarning($"{obj.name} ȱ��Animator���");
                continue;
            }

            if (!HasBoolParameter("action", currentAnim))
            {
                if (showLogs) Debug.LogWarning($"{obj.name} ȱ��action����");
                continue;
            }

            // ���ǰ����
            currentAnim.SetBool("action", true);
            if (showLogs) Debug.Log($"���� {obj.name} ��action");

            // ����ǵ�һ�����屣�ּ�¼
            if (obj == firstValidObject)
            {
                firstAnimator = currentAnim;
            }

            // �ȴ����
            yield return new WaitForSeconds(interval);

            // �����һ������ʱ�ر�
            if (obj != targets[targets.Count - 1])
            {
                currentAnim.SetBool("action", false);
                if (showLogs) Debug.Log($"�ر� {obj.name} ��action");
            }
        }

        // ���ر��׸�����
        if (firstAnimator != null)
        {
            firstAnimator.SetBool("action", false);
            if (showLogs) Debug.Log("�ر��׸������action");
        }

        isPlaying = false;
    }

    // ��֤Animator����
    bool HasBoolParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool)
                return true;
        }
        return false;
    }
}