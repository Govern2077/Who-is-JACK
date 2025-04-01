using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI.Examples;

public class Giveback : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public GameObject objectC;
    public GameObject objectD;

    public List<GameObject> changingObjects;   // 变化的物体列表
    public List<Vector3> objectATargetScales; // 新增：对应不同物品交还情况下objectA的目标缩放值
    public List<GameObject> dialogueObjects;   // 对话物体列表

    private RotateObjectWithMouse rotateObjectWithMouse;
    private MeshRenderer meshRendererB;
    private BoxCollider boxColliderB;
    private MoveObjectOnEsc moveObjectOnEsc;
    private RightClickEffect rightClickEffect;
    private bool previousChangeValue;
    private ObjectAnimator objectAnimator;

    void Start()
    {
        if (objectA != null)
        {
            rotateObjectWithMouse = objectA.GetComponent<RotateObjectWithMouse>();
            Debug.Log("物体A的RotateObjectWithMouse脚本已成功获取");
        }

        if (objectB != null)
        {
            meshRendererB = objectB.GetComponent<MeshRenderer>();
            boxColliderB = objectB.GetComponent<BoxCollider>();
            Debug.Log("物体B的MeshRenderer和BoxCollider已成功获取");
        }

        if (objectC != null)
        {
            moveObjectOnEsc = objectC.GetComponent<MoveObjectOnEsc>();
            Debug.Log("物体C的MoveObjectOnEsc脚本已成功获取");
        }

        if (objectD != null)
        {
            rightClickEffect = objectD.GetComponent<RightClickEffect>();
            Debug.Log("物体D的RightClickEffect脚本已成功获取");
        }

        if (objectA != null)
        {
            objectAnimator = objectA.GetComponent<ObjectAnimator>();
            Debug.Log("物体A的ObjectAnimator脚本已成功获取");
        }

        if (rotateObjectWithMouse != null)
        {
            previousChangeValue = rotateObjectWithMouse.change;
            Debug.Log("初始change值: " + previousChangeValue);
            UpdateMeshRendererState(rotateObjectWithMouse.change);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                if (rotateObjectWithMouse != null && rotateObjectWithMouse.change)
                {
                    rotateObjectWithMouse.change = false;
                    Debug.Log("点击了物体，change已设置为false");

                    // 获取当前激活物体的索引
                    int activeIndex = GetActiveChangingObjectIndex();
                    Vector3 targetScale = activeIndex >= 0 && activeIndex < objectATargetScales.Count
                        ? objectATargetScales[activeIndex]
                        : new Vector3(1, 1, 1); // 默认缩放值

                    // 启动协程来平滑物体A的transform变化
                    StartCoroutine(SmoothTransform(
                        objectA.transform,
                        new Vector3(2, 2, 2),
                        new Vector3(-30, 90, 0),
                        targetScale, // 使用对应物品的目标缩放值
                        0.5f
                    ));

                    CheckAndActivateDialogue();

                    if (objectAnimator != null)
                    {
                        objectAnimator.hasClicked = false;
                        Debug.Log("ObjectAnimator脚本中的hasClicked已设置为false");
                    }
                }
            }
        }

        if (rotateObjectWithMouse != null && rotateObjectWithMouse.change != previousChangeValue)
        {
            Debug.Log("change值发生变化！新的change值: " + rotateObjectWithMouse.change);
            UpdateMeshRendererState(rotateObjectWithMouse.change);
            previousChangeValue = rotateObjectWithMouse.change;
        }

        bool shouldEnableCollider = rotateObjectWithMouse != null && rotateObjectWithMouse.change &&
                                 moveObjectOnEsc != null && !moveObjectOnEsc.look &&
                                 rightClickEffect != null && !rightClickEffect.white && !rightClickEffect.isChanging;

        if (boxColliderB != null)
        {
            boxColliderB.enabled = shouldEnableCollider;
            if (shouldEnableCollider) Debug.Log("物体B的BoxCollider已启用");
        }
    }

    // 新增方法：获取当前激活的changingObjects索引
    private int GetActiveChangingObjectIndex()
    {
        for (int i = 0; i < changingObjects.Count; i++)
        {
            if (changingObjects[i] != null)
            {
                MeshRenderer renderer = changingObjects[i].GetComponent<MeshRenderer>();
                if (renderer != null && renderer.enabled)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private void CheckAndActivateDialogue()
    {
        for (int i = 0; i < changingObjects.Count; i++)
        {
            if (changingObjects[i] != null)
            {
                MeshRenderer renderer = changingObjects[i].GetComponent<MeshRenderer>();
                if (renderer != null && renderer.enabled)
                {
                    if (i < dialogueObjects.Count && dialogueObjects[i] != null)
                    {
                        dialogueObjects[i].SetActive(true);
                        Debug.Log($"激活了对话物体: {dialogueObjects[i].name}");

                        ExampleEvents exampleEvents = dialogueObjects[i].GetComponent<ExampleEvents>();
                        if (exampleEvents != null)
                        {
                            exampleEvents.RestartDialogue();
                            Debug.Log($"调用了对话物体 {dialogueObjects[i].name} 的RestartDialogue方法");
                        }
                    }
                }
            }
        }
    }

    private void UpdateMeshRendererState(bool changeValue)
    {
        if (meshRendererB != null)
        {
            meshRendererB.enabled = changeValue;
            Debug.Log(changeValue ? "启用物体B的MeshRenderer" : "禁用物体B的MeshRenderer");
        }
    }

    private IEnumerator SmoothTransform(Transform targetTransform, Vector3 targetPosition,
                                     Vector3 targetEulerAngles, Vector3 targetScale, float duration)
    {
        Vector3 startPosition = targetTransform.position;
        Quaternion startRotation = targetTransform.rotation;
        Vector3 startScale = targetTransform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            targetTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            targetTransform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetEulerAngles), progress);
            targetTransform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetTransform.position = targetPosition;
        targetTransform.rotation = Quaternion.Euler(targetEulerAngles);
        targetTransform.localScale = targetScale;
    }
}