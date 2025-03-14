using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giveback : MonoBehaviour
{
    public GameObject objectA;  // ����A������A����RotateObjectWithMouse�ű���ObjectAnimator�ű���
    public GameObject objectB;  // ����B������Ҫ������MeshRenderer��BoxCollider��
    public GameObject objectC;  // ����C������MoveObjectOnEsc�ű���
    public GameObject objectD;  // ����D������RightClickEffect�ű���

    private RotateObjectWithMouse rotateObjectWithMouse;  // �����洢����A�ϵ�RotateObjectWithMouse�ű�
    private MeshRenderer meshRendererB;  // ������������B��MeshRenderer
    private BoxCollider boxColliderB;  // ������������B��BoxCollider
    private MoveObjectOnEsc moveObjectOnEsc;  // �����洢����C�ϵ�MoveObjectOnEsc�ű�
    private RightClickEffect rightClickEffect;  // �����洢����D�ϵ�RightClickEffect�ű�
    private bool previousChangeValue;  // �洢��һ�ε�changeֵ���������仯
    private ObjectAnimator objectAnimator;  // �����洢����A�ϵ�ObjectAnimator�ű�

    void Start()
    {
        // ��ȡ����A�ϸ��ӵ� RotateObjectWithMouse �ű�
        if (objectA != null)
        {
            rotateObjectWithMouse = objectA.GetComponent<RotateObjectWithMouse>();
            Debug.Log("����A��RotateObjectWithMouse�ű��ѳɹ���ȡ");
        }

        // ��ȡ����B�ϵ�MeshRenderer�����BoxCollider���
        if (objectB != null)
        {
            meshRendererB = objectB.GetComponent<MeshRenderer>();  // ��ȡ����B��MeshRenderer���
            boxColliderB = objectB.GetComponent<BoxCollider>();  // ��ȡ����B��BoxCollider���
            Debug.Log("����B��MeshRenderer��BoxCollider�ѳɹ���ȡ");
        }

        // ��ȡ����C�ϵ�MoveObjectOnEsc�ű�
        if (objectC != null)
        {
            moveObjectOnEsc = objectC.GetComponent<MoveObjectOnEsc>();
            Debug.Log("����C��MoveObjectOnEsc�ű��ѳɹ���ȡ");
        }

        // ��ȡ����D�ϵ�RightClickEffect�ű�
        if (objectD != null)
        {
            rightClickEffect = objectD.GetComponent<RightClickEffect>();
            Debug.Log("����D��RightClickEffect�ű��ѳɹ���ȡ");
        }

        // ��ȡ����A�ϵ�ObjectAnimator�ű�
        if (objectA != null)
        {
            objectAnimator = objectA.GetComponent<ObjectAnimator>();
            Debug.Log("����A��ObjectAnimator�ű��ѳɹ���ȡ");
        }

        // ��ʼ��previousChangeValue
        if (rotateObjectWithMouse != null)
        {
            previousChangeValue = rotateObjectWithMouse.change;
            Debug.Log("��ʼchangeֵ: " + previousChangeValue);
            UpdateMeshRendererState(rotateObjectWithMouse.change); // ��ʼ״̬
        }
    }

    void Update()
    {
        // ����Ƿ����˵�ǰ����
        if (Input.GetMouseButtonDown(0))  // ������
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // �����λ�÷�������
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))  // ���������������
            {
                if (hit.collider.gameObject == gameObject)  // ���������ǵ�ǰ����
                {
                    // ֻ��change��true��Ϊfalse
                    if (rotateObjectWithMouse != null && rotateObjectWithMouse.change)
                    {
                        rotateObjectWithMouse.change = false;
                        Debug.Log("��������壬change������Ϊfalse");

                        // ����Э����ƽ������A��transform�仯
                        StartCoroutine(SmoothTransform(objectA.transform, new Vector3(2, 2, 2), new Vector3(-30, 90, 0), new Vector3(1, 1, 1), 0.5f));

                        // ����ObjectAnimator�ű��е�hasClickedΪfalse
                        if (objectAnimator != null)
                        {
                            objectAnimator.hasClicked = false;
                            Debug.Log("ObjectAnimator�ű��е�hasClicked������Ϊfalse");
                        }
                    }
                }
            }
        }

        // ���changeֵ�ı仯����������B��MeshRenderer����ʾ״̬
        if (rotateObjectWithMouse != null && rotateObjectWithMouse.change != previousChangeValue)
        {
            // ���change��ֵ�����仯����������B��MeshRenderer����ʾ״̬
            Debug.Log("changeֵ�����仯���µ�changeֵ: " + rotateObjectWithMouse.change);
            UpdateMeshRendererState(rotateObjectWithMouse.change);
            previousChangeValue = rotateObjectWithMouse.change;  // ������һ�ε�changeֵ
        }

        // ����Ƿ�������������������B��BoxCollider
        if (rotateObjectWithMouse != null && rotateObjectWithMouse.change &&
            moveObjectOnEsc != null && !moveObjectOnEsc.look &&
            rightClickEffect != null && !rightClickEffect.white && !rightClickEffect.isChanging)
        {
            // ���������������������B��BoxCollider
            if (boxColliderB != null)
            {
                boxColliderB.enabled = true;
                Debug.Log("����B��BoxCollider������");
            }
           
        }
        else
        {
            // ֻҪ���κ�һ�����������㣬��������B��BoxCollider
            if (boxColliderB != null)
            {
                boxColliderB.enabled = false;
                Debug.Log("����B��BoxCollider�ѽ���");
            }
        }
    }

    // ����changeֵ��������B��MeshRenderer����ʾ״̬
    private void UpdateMeshRendererState(bool changeValue)
    {
        if (meshRendererB != null)
        {
            if (changeValue)
            {
                Debug.Log("��������B��MeshRenderer");
            }
            else
            {
                Debug.Log("��������B��MeshRenderer");
            }

            meshRendererB.enabled = changeValue;  // ���changeΪtrue����ʾMeshRenderer����������
        }
    }

    // ƽ�����޸�����A��λ�á���ת������
    private IEnumerator SmoothTransform(Transform targetTransform, Vector3 targetPosition, Vector3 targetEulerAngles, Vector3 targetScale, float duration)
    {
        Vector3 startPosition = targetTransform.position;
        Vector3 startEulerAngles = targetTransform.eulerAngles;  // ��ȡ����ĳ�ʼŷ����
        Vector3 startScale = targetTransform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ƽ������λ��
            targetTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            // ƽ��������ת�Ƕ�
            targetTransform.eulerAngles = Vector3.Lerp(startEulerAngles, targetEulerAngles, elapsedTime / duration);

            // ƽ����������
            targetTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;  // �ȴ���һ֡
        }

        // ȷ������ֵ������
        targetTransform.position = targetPosition;
        targetTransform.eulerAngles = targetEulerAngles;  // ȷ���������յ�ŷ����
        targetTransform.localScale = targetScale;
    }
}
