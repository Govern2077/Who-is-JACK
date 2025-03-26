using System;
using System.Collections;
using UnityEngine;

public class ObjectAnimator : MonoBehaviour
{
    public Transform objectA; // 物体A的Transform
    private bool isAnimating = false; // 标记是否正在进行动画
    public bool  hasClicked = false;  // 标记是否已经点击过

    void Start()
    {
        // 通过EventCenter订阅"change"事件
        EventCenter.Instance.Subscribe("change", OnChangeEventTriggered);
    }

    void Update()
    {
        // 只在第一次点击时触发动画
        if (!hasClicked && Input.GetMouseButtonDown(0)) // 0 是鼠标左键
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 如果点击的是物体A，则触发change事件
                if (hit.transform == objectA)
                {
                    Debug.Log("Triggering 'change' event.");
                    EventCenter.Instance.TriggerEvent("change");

                    // 标记已经点击过，确保以后不会再触发
                    hasClicked = true;
                }
            }
        }

        // 示例：你可以在动画结束后控制物体的旋转和位置
        if (!isAnimating)
        {
            // 可以在此处加入代码来继续控制物体的旋转和位置
            // 例如：物体旋转
            //objectA.Rotate(Vector3.up * 20f * TimeTime); // 每秒旋转20度
            // 物体的位置
            // objectA.position += Vector3.forward * TimeTime; // 每秒向前移动
        }
    }

    // 当change事件触发时，调用这个方法
    private void OnChangeEventTriggered()
    {
        // 开始协程执行平滑过渡动画
        if (!isAnimating) // 确保不重复触发动画
        {
            isAnimating = true;
            StartCoroutine(AnimateObject());
        }
    }

    // 协程：在0.5秒内将物体A的scale, rotation, position平滑过渡到目标值
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

        // 平滑过渡
        while (t < duration)
        {
            t += Time.deltaTime;
            float pct = t / duration;

            // 使用Lerp来平滑过渡
            objectA.localScale = Vector3.Lerp(initialScale, targetScale, pct);
            objectA.rotation = Quaternion.Lerp(initialRotation, targetRotation, pct);
            objectA.position = Vector3.Lerp(initialPosition, targetPosition, pct);

            yield return null;
        }

        // 确保最终达到目标值
        objectA.localScale = targetScale;
        objectA.rotation = targetRotation;
        objectA.position = targetPosition;

        // 动画结束后，可以控制物体的旋转和位置
        isAnimating = false; // 动画完成，允许继续控制物体
    }
}
