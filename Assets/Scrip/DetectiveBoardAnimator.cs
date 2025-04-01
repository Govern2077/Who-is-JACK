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

    [Header("动画序列配置")]
    public List<AnimationSequence> animationSequences = new List<AnimationSequence>();

    [Header("触发设置")]
    [SerializeField] int targetSequenceID = 0;
    [SerializeField] float stepDuration = 3f;
    [SerializeField] bool destroyOnComplete = true; // 新增销毁开关

    [Header("调试设置")]
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
            Debug.LogError("无效的动画序列配置");
            yield break;
        }

        // 获取第一个有效Animator
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
            Debug.LogWarning("未找到有效的起始Animator");
            isPlaying = false;
            yield break;
        }

        // 执行动画序列
        for (int i = 0; i < sequence.targetObjects.Count; i++)
        {
            GameObject currentObj = sequence.targetObjects[i];
            if (currentObj == null) continue;

            if (currentObj.TryGetComponent<Animator>(out Animator currentAnim) && HasBoolParameter("action", currentAnim))
            {
                currentAnim.SetBool("action", true);
                if (debugMode) Debug.Log($"激活 {currentObj.name} 的action");

                if (i < sequence.targetObjects.Count)
                {
                    yield return new WaitForSeconds(stepDuration);
                }
            }
            else if (debugMode)
            {
                Debug.LogWarning($"{currentObj?.name ?? "空物体"} 动画参数无效");
            }
        }

        // 关闭第一个物体的action
        if (firstAnimator != null)
        {
            firstAnimator.SetBool("action", false);
            if (debugMode) Debug.Log("关闭首个物体的action");
        }

        // 销毁自身物体（新增部分）
        if (destroyOnComplete)
        {
            if (debugMode) Debug.Log($"动画完成，销毁 {gameObject.name}");
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