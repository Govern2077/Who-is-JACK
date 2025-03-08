using Flockaroo;
using System.Collections.Generic;
using UnityEngine;

public class RightClickEffect : MonoBehaviour
{
    private ColoredPencilsEffect coloredPencilsEffect;  // 引用 ColoredPencilsEffect 脚本
    private List<Outline> outlineObjects;  // 存储所有物体的 Outline 脚本
    private bool isChanging = false;  // 判断是否正在进行数值过渡
    private bool isReversed = false;  // 判断是否已经反转了值
    private float transitionTime = 0.5f;  // 数值变化的过渡时间
    private float currentTime = 0f;  // 当前过渡的时间

    private void Start()
    {
        // 获取 Main Camera 上的 ColoredPencilsEffect 脚本
        coloredPencilsEffect = Camera.main.GetComponent<ColoredPencilsEffect>();

        // 获取所有物体的 Outline 脚本
        outlineObjects = new List<Outline>(FindObjectsOfType<Outline>());
    }

    private void Update()
    {
        // 检测鼠标右键按下
        if (Input.GetMouseButtonDown(1) && !isChanging)
        {
            // 防止在过渡过程中重复触发
            StartCoroutine(ChangeValues());
        }
    }

    private System.Collections.IEnumerator ChangeValues()
    {
        isChanging = true;  // 设置正在进行过渡

        // 判断是否是第一次变化
        if (!isReversed)
        {
            // 第一次按下，开始过渡
            float startOutline = coloredPencilsEffect.outlines;
            float startHatch = coloredPencilsEffect.hatches;

            // 目标值分别是 0 和 0.5
            float targetOutline = 0f;
            float targetHatch = 0.5f;

            // 获取所有物体的当前 outlineWidth 和目标值
            float startOutlineWidth = outlineObjects[0].OutlineWidth;
            float targetOutlineWidth = 10f;

            // 平滑过渡
            while (currentTime < transitionTime)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / transitionTime;

                // 改变 ColoredPencilsEffect 中的 outlines 和 hatches
                coloredPencilsEffect.outlines = Mathf.Lerp(startOutline, targetOutline, t);
                coloredPencilsEffect.hatches = Mathf.Lerp(startHatch, targetHatch, t);

                // 改变所有物体的 outlineWidth
                foreach (Outline outline in outlineObjects)
                {
                    outline.OutlineWidth = Mathf.Lerp(startOutlineWidth, targetOutlineWidth, t);
                }

                yield return null;
            }

            // 最终值设置为 0 和 0.5
            coloredPencilsEffect.outlines = targetOutline;
            coloredPencilsEffect.hatches = targetHatch;

            // 设置所有物体的最终 outlineWidth
            foreach (Outline outline in outlineObjects)
            {
                outline.OutlineWidth = targetOutlineWidth;
            }

            // 设置已反转标记，下一次按下将会恢复
            isReversed = true;
        }
        else
        {
            // 第二次按下，恢复原值
            float startOutline = coloredPencilsEffect.outlines;
            float startHatch = coloredPencilsEffect.hatches;

            // 目标值分别是 1 和 1
            float targetOutline = 1f;
            float targetHatch = 1f;

            // 获取所有物体的当前 outlineWidth 和目标值
            float startOutlineWidth = outlineObjects[0].OutlineWidth;
            float targetOutlineWidth = 0f;

            // 平滑过渡
            while (currentTime < transitionTime)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / transitionTime;

                // 改变 ColoredPencilsEffect 中的 outlines 和 hatches
                coloredPencilsEffect.outlines = Mathf.Lerp(startOutline, targetOutline, t);
                coloredPencilsEffect.hatches = Mathf.Lerp(startHatch, targetHatch, t);

                // 改变所有物体的 outlineWidth
                foreach (Outline outline in outlineObjects)
                {
                    outline.OutlineWidth = Mathf.Lerp(startOutlineWidth, targetOutlineWidth, t);
                }

                yield return null;
            }

            // 最终值设置为 1 和 1
            coloredPencilsEffect.outlines = targetOutline;
            coloredPencilsEffect.hatches = targetHatch;

            // 设置所有物体的最终 outlineWidth
            foreach (Outline outline in outlineObjects)
            {
                outline.OutlineWidth = targetOutlineWidth;
            }

            // 重置标记，准备下一次反转
            isReversed = false;
        }

        // 重置过渡时间，准备下一次操作
        currentTime = 0f;
        isChanging = false;  // 允许再次进行操作
    }
}
