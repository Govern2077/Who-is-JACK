using Flockaroo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickEffect : MonoBehaviour
{
    private ColoredPencilsEffect coloredPencilsEffect;  // 引用 ColoredPencilsEffect 脚本
    private List<Outline> outlineObjects;  // 存储所有物体的 Outline 脚本
    public bool isChanging = false;  // 判断是否正在进行数值过渡
    private float transitionTime = 0.5f;  // 数值变化的过渡时间
    private float currentTime = 0f;  // 当前过渡的时间

    // 手动拖动物体 A 中的 RotateObjectWithMouse 脚本到此变量
    public RotateObjectWithMouse rotateObjectWithMouse;  // 引用 RotateObjectWithMouse 脚本

    // public 用于检测 outline.OutlineWidth 是否不为 0
    public bool white = false;

    // 用于存储多个需要平滑过渡透明度的物体的材质
    public List<Renderer> transparentObjectRenderers;  // 需要平滑过渡透明度的物体列表

    // 引用 MoveObjectOnEsc 脚本
    public MoveObjectOnEsc moveObjectOnEsc;  // 引用 MoveObjectOnEsc 脚本

    private void Start()
    {
        // 获取 Main Camera 上的 ColoredPencilsEffect 脚本
        coloredPencilsEffect = Camera.main.GetComponent<ColoredPencilsEffect>();

        // 获取所有物体的 Outline 脚本
        outlineObjects = new List<Outline>(FindObjectsOfType<Outline>());

        // 订阅事件，使用匿名函数启动协程
        EventCenter.Instance.Subscribe("white", () => StartCoroutine(ChangeValues()));
    }

    private void Update()
    {
        // 检查 moveObjectOnEsc 脚本中的 look 是否为 false，才执行右键点击事件
        if (moveObjectOnEsc != null && !moveObjectOnEsc.look 
            && rotateObjectWithMouse != null 
            && rotateObjectWithMouse.change 
            && Input.GetMouseButtonDown(1) 
            && !isChanging)
        {
            // 触发 white 事件
            EventCenter.Instance.TriggerEvent("white");
        }
    }

    private IEnumerator ChangeValues()
    {
        if (isChanging)
            yield break;
        isChanging = true;  // 设置正在进行过渡

        // 获取当前的 outlines 和 hatches 值
        float startOutline = coloredPencilsEffect.outlines;
        float startHatch = coloredPencilsEffect.hatches;

        // 获取所有物体的当前 outlineWidth 和目标值
        float startOutlineWidth = outlineObjects[0].OutlineWidth;

        // 设置目标值的初始值
        float targetOutline = startOutline;
        float targetHatch = startHatch;
        float targetOutlineWidth = startOutlineWidth;

        // 根据当前值决定变换的目标值
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

        // 存储每个物体的当前透明度值
        List<float> startAlphas = new List<float>();
        foreach (Renderer transparentObject in transparentObjectRenderers)
        {
            Color startColor = transparentObject.material.GetColor("_Color");
            startAlphas.Add(startColor.a);
        }

        // 计算目标透明度
        List<float> targetAlphas = new List<float>();
        foreach (float startAlpha in startAlphas)
        {
            targetAlphas.Add(startAlpha == 0f ? 1f : 0f);  // 0 -> 1, 1 -> 0
        }

        // 平滑过渡
        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;  // 正确获取时间增量
            float t = currentTime / transitionTime;

            // 改变 ColoredPencilsEffect 中的 outlines 和 hatches
            coloredPencilsEffect.outlines = Mathf.Lerp(startOutline, targetOutline, t);
            coloredPencilsEffect.hatches = Mathf.Lerp(startHatch, targetHatch, t);

            // 改变所有物体的 outlineWidth
            foreach (Outline outline in outlineObjects)
            {
                outline.OutlineWidth = Mathf.Lerp(startOutlineWidth, targetOutlineWidth, t);
            }

            // 改变每个透明物体的透明度
            for (int i = 0; i < transparentObjectRenderers.Count; i++)
            {
                Renderer transparentObject = transparentObjectRenderers[i];
                float startAlpha = startAlphas[i];
                float targetAlpha = targetAlphas[i];

                // 平滑透明度变化
                Color newColor = new Color(transparentObject.material.color.r, transparentObject.material.color.g, transparentObject.material.color.b, Mathf.Lerp(startAlpha, targetAlpha, t));
                transparentObject.material.SetColor("_Color", newColor);
            }

            yield return null;
        }

        // 最终值设置为目标值
        coloredPencilsEffect.outlines = targetOutline;
        coloredPencilsEffect.hatches = targetHatch;

        // 设置所有物体的最终 outlineWidth
        foreach (Outline outline in outlineObjects)
        {
            outline.OutlineWidth = targetOutlineWidth;
        }

        // 设置每个透明物体的最终透明度
        for (int i = 0; i < transparentObjectRenderers.Count; i++)
        {
            Renderer transparentObject = transparentObjectRenderers[i];
            float targetAlpha = targetAlphas[i];
            Color finalColor = new Color(transparentObject.material.color.r, transparentObject.material.color.g, transparentObject.material.color.b, targetAlpha);
            transparentObject.material.SetColor("_Color", finalColor);
        }

        // 设置 white 布尔值，检测 outline.OutlineWidth 是否不为 0
        white = (outlineObjects[0].OutlineWidth != 0f);

        // 重置过渡时间，准备下一次操作
        currentTime = 0f;
        isChanging = false;  // 允许再次进行操作
    }

    private void OnDestroy()
    {
        // 确保在销毁时取消订阅
        EventCenter.Instance.Unsubscribe("white", () => StartCoroutine(ChangeValues()));
    }
}
