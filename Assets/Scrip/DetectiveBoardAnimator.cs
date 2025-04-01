using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SequentialAnimationController : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSequence
    {
        public int sequenceID;
        public List<GameObject> targetObjects;
    }

    [Header("������������")]
    public List<AnimationSequence> animationSequences = new List<AnimationSequence>();

    [Header("��������")]
    [SerializeField] int targetSequenceID = 0;
    [SerializeField] float stepDuration = 3f;
    [SerializeField] bool destroyOnComplete = true; // �������ٿ���

    [Header("��������")]
    [SerializeField] bool debugMode = true;

    private bool isPlaying;
    private Animator firstAnimator;

    void OnMouseDown()
    {
        if (!isPlaying)
        {
            StartCoroutine(PlayAnimationSequence());
        }
    }

    IEnumerator PlayAnimationSequence()
    {
        isPlaying = true;

        AnimationSequence sequence = animationSequences.Find(s => s.sequenceID == targetSequenceID);
        if (sequence == null || sequence.targetObjects.Count == 0)
        {
            Debug.LogError("��Ч�Ķ�����������");
            yield break;
        }

        // ��ȡ��һ����ЧAnimator
        firstAnimator = null;
        foreach (GameObject obj in sequence.targetObjects)
        {
            if (obj != null && obj.TryGetComponent<Animator>(out Animator anim) && HasBoolParameter("action", anim))
            {
                firstAnimator = anim;
                break;
            }
        }

        if (firstAnimator == null)
        {
            Debug.LogWarning("δ�ҵ���Ч����ʼAnimator");
            isPlaying = false;
            yield break;
        }

        // ִ�ж�������
        for (int i = 0; i < sequence.targetObjects.Count; i++)
        {
            GameObject currentObj = sequence.targetObjects[i];
            if (currentObj == null) continue;

            if (currentObj.TryGetComponent<Animator>(out Animator currentAnim) && HasBoolParameter("action", currentAnim))
            {
                currentAnim.SetBool("action", true);
                if (debugMode) Debug.Log($"���� {currentObj.name} ��action");

                if (i < sequence.targetObjects.Count)
                {
                    yield return new WaitForSeconds(stepDuration);
                }
            }
            else if (debugMode)
            {
                Debug.LogWarning($"{currentObj?.name ?? "������"} ����������Ч");
            }
        }

        // �رյ�һ�������action
        if (firstAnimator != null)
        {
            firstAnimator.SetBool("action", false);
            if (debugMode) Debug.Log("�ر��׸������action");
        }

        // �����������壨�������֣�
        if (destroyOnComplete)
        {
            if (debugMode) Debug.Log($"������ɣ����� {gameObject.name}");
            Destroy(gameObject);
        }

        isPlaying = false;
    }

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