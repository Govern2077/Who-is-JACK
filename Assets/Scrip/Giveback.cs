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

    public List<GameObject> changingObjects;   // �仯�������б�
    public List<Vector3> objectATargetScales; // ��������Ӧ��ͬ��Ʒ���������objectA��Ŀ������ֵ
    public List<GameObject> dialogueObjects;   // �Ի������б�

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
            Debug.Log("����A��RotateObjectWithMouse�ű��ѳɹ���ȡ");
        }

        if (objectB != null)
        {
            meshRendererB = objectB.GetComponent<MeshRenderer>();
            boxColliderB = objectB.GetComponent<BoxCollider>();
            Debug.Log("����B��MeshRenderer��BoxCollider�ѳɹ���ȡ");
        }

        if (objectC != null)
        {
            moveObjectOnEsc = objectC.GetComponent<MoveObjectOnEsc>();
            Debug.Log("����C��MoveObjectOnEsc�ű��ѳɹ���ȡ");
        }

        if (objectD != null)
        {
            rightClickEffect = objectD.GetComponent<RightClickEffect>();
            Debug.Log("����D��RightClickEffect�ű��ѳɹ���ȡ");
        }

        if (objectA != null)
        {
            objectAnimator = objectA.GetComponent<ObjectAnimator>();
            Debug.Log("����A��ObjectAnimator�ű��ѳɹ���ȡ");
        }

        if (rotateObjectWithMouse != null)
        {
            previousChangeValue = rotateObjectWithMouse.change;
            Debug.Log("��ʼchangeֵ: " + previousChangeValue);
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
                    Debug.Log("��������壬change������Ϊfalse");

                    // ��ȡ��ǰ�������������
                    int activeIndex = GetActiveChangingObjectIndex();
                    Vector3 targetScale = activeIndex >= 0 && activeIndex < objectATargetScales.Count
                        ? objectATargetScales[activeIndex]
                        : new Vector3(1, 1, 1); // Ĭ������ֵ

                    // ����Э����ƽ������A��transform�仯
                    StartCoroutine(SmoothTransform(
                        objectA.transform,
                        new Vector3(2, 2, 2),
                        new Vector3(-30, 90, 0),
                        targetScale, // ʹ�ö�Ӧ��Ʒ��Ŀ������ֵ
                        0.5f
                    ));

                    CheckAndActivateDialogue();

                    if (objectAnimator != null)
                    {
                        objectAnimator.hasClicked = false;
                        Debug.Log("ObjectAnimator�ű��е�hasClicked������Ϊfalse");
                    }
                }
            }
        }

        if (rotateObjectWithMouse != null && rotateObjectWithMouse.change != previousChangeValue)
        {
            Debug.Log("changeֵ�����仯���µ�changeֵ: " + rotateObjectWithMouse.change);
            UpdateMeshRendererState(rotateObjectWithMouse.change);
            previousChangeValue = rotateObjectWithMouse.change;
        }

        bool shouldEnableCollider = rotateObjectWithMouse != null && rotateObjectWithMouse.change &&
                                 moveObjectOnEsc != null && !moveObjectOnEsc.look &&
                                 rightClickEffect != null && !rightClickEffect.white && !rightClickEffect.isChanging;

        if (boxColliderB != null)
        {
            boxColliderB.enabled = shouldEnableCollider;
            if (shouldEnableCollider) Debug.Log("����B��BoxCollider������");
        }
    }

    // ������������ȡ��ǰ�����changingObjects����
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
                        Debug.Log($"�����˶Ի�����: {dialogueObjects[i].name}");

                        ExampleEvents exampleEvents = dialogueObjects[i].GetComponent<ExampleEvents>();
                        if (exampleEvents != null)
                        {
                            exampleEvents.RestartDialogue();
                            Debug.Log($"�����˶Ի����� {dialogueObjects[i].name} ��RestartDialogue����");
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
            Debug.Log(changeValue ? "��������B��MeshRenderer" : "��������B��MeshRenderer");
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