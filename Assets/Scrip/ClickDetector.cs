using Febucci.UI.Examples;
using UnityEngine;
using System.Collections;

public class ClickDetector : MonoBehaviour
{
    [Header("关联的对话控制器")]
    [SerializeField]
    public ExampleEvents exampleEvents; // 拖拽绑定 ExampleEvents 物体

    [Header("禁用按键检测的持续时间")]
    [SerializeField]
    private float disableDuration = 1f; // 默认禁用2秒

    // 当鼠标点击该物体的Collider时触发
    void OnMouseDown()
    {
        Debug.Log($"点击了物体：{gameObject.name}");

        // 禁用对话继续
        if (exampleEvents != null)
        {
            exampleEvents.canContinue = false;
            StartCoroutine(EnableContinueAfterDelay());
        }
    }

    // 协程：在指定时间后恢复允许继续对话
    IEnumerator EnableContinueAfterDelay()
    {
        yield return new WaitForSeconds(disableDuration);
        exampleEvents.canContinue = true;
        Debug.Log("已重新允许继续对话");
    }
}