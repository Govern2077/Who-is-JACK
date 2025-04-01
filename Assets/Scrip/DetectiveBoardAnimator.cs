using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))] // 确保挂载碰撞器
public class DetectiveBoardTrigger : MonoBehaviour
{
    [System.Serializable]
    public class PointPair
    {
        public int index;
        public List<GameObject> targetObjects; // 改名为更清晰的变量名
    }

    [Header("动画配置")]
    [Tooltip("侦探版动画序列配置")]
    public List<PointPair> positionPairs = new List<PointPair>();

    [Header("触发设置")]
    [SerializeField] string triggerTag = "Player"; // 只响应特定标签的物体
    [SerializeField] int targetIndex = 0;          // 要触发的PointPair索引

    [Header("动画参数")]
    [SerializeField] float interval = 3f;         // 可配置间隔时间

    [Header("调试模式")]
    [SerializeField] bool showLogs = true;         // 日志开关

    private bool isPlaying = false;                // 防止重复触发

    // 初始化时验证组件
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"物体 {name} 的碰撞器未设置为触发器，已自动启用");
            col.isTrigger = true;
        }
    }

    // 碰撞触发检测
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
            if (showLogs) Debug.Log("动画正在播放中，忽略新请求");
            return;
        }

        PointPair targetPair = positionPairs.Find(p => p.index == targetIndex);
        if (targetPair == null)
        {
            Debug.LogError($"未找到索引为 {targetIndex} 的PointPair配置");
            return;
        }

        StartCoroutine(ProcessAnimationSequence(targetPair.targetObjects));
        isPlaying = true;
    }

    IEnumerator ProcessAnimationSequence(List<GameObject> targets)
    {
        if (targets.Count == 0)
        {
            if (showLogs) Debug.LogWarning("目标物体列表为空");
            yield break;
        }

        Animator firstAnimator = null;
        GameObject firstValidObject = null;

        // 第一阶段：寻找首个有效物体
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
            Debug.LogWarning("未找到有效起始物体");
            yield break;
        }

        // 第二阶段：顺序激活动画
        foreach (GameObject obj in targets)
        {
            if (obj == null)
            {
                if (showLogs) Debug.LogWarning("检测到空物体引用，跳过");
                continue;
            }

            Animator currentAnim = obj.GetComponent<Animator>();
            if (currentAnim == null)
            {
                if (showLogs) Debug.LogWarning($"{obj.name} 缺少Animator组件");
                continue;
            }

            if (!HasBoolParameter("action", currentAnim))
            {
                if (showLogs) Debug.LogWarning($"{obj.name} 缺少action参数");
                continue;
            }

            // 激活当前物体
            currentAnim.SetBool("action", true);
            if (showLogs) Debug.Log($"激活 {obj.name} 的action");

            // 如果是第一个物体保持记录
            if (obj == firstValidObject)
            {
                firstAnimator = currentAnim;
            }

            // 等待间隔
            yield return new WaitForSeconds(interval);

            // 非最后一个物体时关闭
            if (obj != targets[targets.Count - 1])
            {
                currentAnim.SetBool("action", false);
                if (showLogs) Debug.Log($"关闭 {obj.name} 的action");
            }
        }

        // 最后关闭首个物体
        if (firstAnimator != null)
        {
            firstAnimator.SetBool("action", false);
            if (showLogs) Debug.Log("关闭首个物体的action");
        }

        isPlaying = false;
    }

    // 验证Animator参数
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