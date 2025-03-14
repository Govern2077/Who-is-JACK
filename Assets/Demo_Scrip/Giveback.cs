using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giveback : MonoBehaviour
{
    public GameObject objectA;  // 物体A（物体A上有RotateObjectWithMouse脚本和ObjectAnimator脚本）
    public GameObject objectB;  // 物体B（我们要控制其MeshRenderer和BoxCollider）
    public GameObject objectC;  // 物体C（含有MoveObjectOnEsc脚本）
    public GameObject objectD;  // 物体D（含有RightClickEffect脚本）

    private RotateObjectWithMouse rotateObjectWithMouse;  // 用来存储物体A上的RotateObjectWithMouse脚本
    private MeshRenderer meshRendererB;  // 用来控制物体B的MeshRenderer
    private BoxCollider boxColliderB;  // 用来控制物体B的BoxCollider
    private MoveObjectOnEsc moveObjectOnEsc;  // 用来存储物体C上的MoveObjectOnEsc脚本
    private RightClickEffect rightClickEffect;  // 用来存储物体D上的RightClickEffect脚本
    private bool previousChangeValue;  // 存储上一次的change值，用来检测变化
    private ObjectAnimator objectAnimator;  // 用来存储物体A上的ObjectAnimator脚本

    void Start()
    {
        // 获取物体A上附加的 RotateObjectWithMouse 脚本
        if (objectA != null)
        {
            rotateObjectWithMouse = objectA.GetComponent<RotateObjectWithMouse>();
            Debug.Log("物体A的RotateObjectWithMouse脚本已成功获取");
        }

        // 获取物体B上的MeshRenderer组件和BoxCollider组件
        if (objectB != null)
        {
            meshRendererB = objectB.GetComponent<MeshRenderer>();  // 获取物体B的MeshRenderer组件
            boxColliderB = objectB.GetComponent<BoxCollider>();  // 获取物体B的BoxCollider组件
            Debug.Log("物体B的MeshRenderer和BoxCollider已成功获取");
        }

        // 获取物体C上的MoveObjectOnEsc脚本
        if (objectC != null)
        {
            moveObjectOnEsc = objectC.GetComponent<MoveObjectOnEsc>();
            Debug.Log("物体C的MoveObjectOnEsc脚本已成功获取");
        }

        // 获取物体D上的RightClickEffect脚本
        if (objectD != null)
        {
            rightClickEffect = objectD.GetComponent<RightClickEffect>();
            Debug.Log("物体D的RightClickEffect脚本已成功获取");
        }

        // 获取物体A上的ObjectAnimator脚本
        if (objectA != null)
        {
            objectAnimator = objectA.GetComponent<ObjectAnimator>();
            Debug.Log("物体A的ObjectAnimator脚本已成功获取");
        }

        // 初始化previousChangeValue
        if (rotateObjectWithMouse != null)
        {
            previousChangeValue = rotateObjectWithMouse.change;
            Debug.Log("初始change值: " + previousChangeValue);
            UpdateMeshRendererState(rotateObjectWithMouse.change); // 初始状态
        }
    }

    void Update()
    {
        // 检查是否点击了当前物体
        if (Input.GetMouseButtonDown(0))  // 左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // 从鼠标位置发射射线
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))  // 如果射线碰到物体
            {
                if (hit.collider.gameObject == gameObject)  // 如果点击的是当前物体
                {
                    // 只将change从true变为false
                    if (rotateObjectWithMouse != null && rotateObjectWithMouse.change)
                    {
                        rotateObjectWithMouse.change = false;
                        Debug.Log("点击了物体，change已设置为false");

                        // 启动协程来平滑物体A的transform变化
                        StartCoroutine(SmoothTransform(objectA.transform, new Vector3(2, 2, 2), new Vector3(-30, 90, 0), new Vector3(1, 1, 1), 0.5f));

                        // 设置ObjectAnimator脚本中的hasClicked为false
                        if (objectAnimator != null)
                        {
                            objectAnimator.hasClicked = false;
                            Debug.Log("ObjectAnimator脚本中的hasClicked已设置为false");
                        }
                    }
                }
            }
        }

        // 检测change值的变化，更新物体B的MeshRenderer的显示状态
        if (rotateObjectWithMouse != null && rotateObjectWithMouse.change != previousChangeValue)
        {
            // 如果change的值发生变化，更新物体B的MeshRenderer的显示状态
            Debug.Log("change值发生变化！新的change值: " + rotateObjectWithMouse.change);
            UpdateMeshRendererState(rotateObjectWithMouse.change);
            previousChangeValue = rotateObjectWithMouse.change;  // 更新上一次的change值
        }

        // 检查是否满足条件来启用物体B的BoxCollider
        if (rotateObjectWithMouse != null && rotateObjectWithMouse.change &&
            moveObjectOnEsc != null && !moveObjectOnEsc.look &&
            rightClickEffect != null && !rightClickEffect.white && !rightClickEffect.isChanging)
        {
            // 如果满足条件，启用物体B的BoxCollider
            if (boxColliderB != null)
            {
                boxColliderB.enabled = true;
                Debug.Log("物体B的BoxCollider已启用");
            }
           
        }
        else
        {
            // 只要有任何一条条件不满足，禁用物体B的BoxCollider
            if (boxColliderB != null)
            {
                boxColliderB.enabled = false;
                Debug.Log("物体B的BoxCollider已禁用");
            }
        }
    }

    // 根据change值更新物体B的MeshRenderer的显示状态
    private void UpdateMeshRendererState(bool changeValue)
    {
        if (meshRendererB != null)
        {
            if (changeValue)
            {
                Debug.Log("启用物体B的MeshRenderer");
            }
            else
            {
                Debug.Log("禁用物体B的MeshRenderer");
            }

            meshRendererB.enabled = changeValue;  // 如果change为true，显示MeshRenderer，否则隐藏
        }
    }

    // 平滑地修改物体A的位置、旋转和缩放
    private IEnumerator SmoothTransform(Transform targetTransform, Vector3 targetPosition, Vector3 targetEulerAngles, Vector3 targetScale, float duration)
    {
        Vector3 startPosition = targetTransform.position;
        Vector3 startEulerAngles = targetTransform.eulerAngles;  // 获取物体的初始欧拉角
        Vector3 startScale = targetTransform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 平滑过渡位置
            targetTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            // 平滑过渡旋转角度
            targetTransform.eulerAngles = Vector3.Lerp(startEulerAngles, targetEulerAngles, elapsedTime / duration);

            // 平滑过渡缩放
            targetTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;  // 等待下一帧
        }

        // 确保最终值被设置
        targetTransform.position = targetPosition;
        targetTransform.eulerAngles = targetEulerAngles;  // 确保设置最终的欧拉角
        targetTransform.localScale = targetScale;
    }
}
