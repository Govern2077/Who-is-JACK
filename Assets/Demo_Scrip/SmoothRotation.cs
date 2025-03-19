using UnityEngine;
using System.Collections;


public class SmoothRotation : MonoBehaviour
{
    private float rotationSpeed = 10f;  // ������ת���ɵ��ٶ�
    private float transitionTime = 1f;  // ��ת�� 0 �����ʱ��
    private MeshRenderer meshRenderer;  // ��ǰ����� MeshRenderer ���
    public GameObject objectA;  // ���� A

    // ������һ������� RightClickEffect �ű�
    public RightClickEffect rightClickEffect;  // ���� RightClickEffect �ű�

    // �������� C
    public GameObject objectC;  // ���� C

    private void Start()
    {
        // ��ȡ�����ϵ� MeshRenderer ���
        meshRenderer = GetComponent<MeshRenderer>();

        // ���� "white" �¼�
        EventCenter.Instance.Subscribe("white", OnWhiteEventTriggered);
    }

    private void Update()
    {
        // �����ǰ����� MeshRenderer ����ǹرյģ�ֱ�ӷ���
        if (meshRenderer != null && !meshRenderer.enabled)
        {
            return;
        }

        // ��ȡ���� C ����ת�Ƕ�
        float currentRotationX = objectC.transform.rotation.eulerAngles.x;
        float currentRotationY = objectC.transform.rotation.eulerAngles.y;

        // ȷ����ת�Ƕ��� -180 �� 180 ֮�䣬�����������
        if (currentRotationX > 180f) currentRotationX -= 360f;
        if (currentRotationY > 180f) currentRotationY -= 360f;

        // �����ת�Ƕ��� -2 �� 2 ֮�䣬��ʼƽ�����ɵ� 0
        if (currentRotationX > -2f && currentRotationX < 2f && currentRotationY > -2f && currentRotationY < 2f && rightClickEffect.white && !rightClickEffect.isChanging)
        {
            // ����һ��Ŀ����ת�Ƕ�
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, objectC.transform.rotation.eulerAngles.z);

            // ʹ�� Slerp ƽ�����ɵ�Ŀ����ת
            objectC.transform.rotation = Quaternion.Slerp(objectC.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / transitionTime);
        }

        // �ж����� C �� rotationX �� rotationY �Ƿ�Ϊ 0������ RightClickEffect �ű��е� white Ϊ true
        if (Mathf.Abs(currentRotationX) < 0.1f && Mathf.Abs(currentRotationY) < 0.1f &&
            rightClickEffect != null && rightClickEffect.white && !rightClickEffect.isChanging)
        {
            // ����������������� "white" �¼�
            EventCenter.Instance.TriggerEvent("white");

            // �ӳ� 0.2 ���ִ�к����߼�
            StartCoroutine(DelayedAction());
        }
    }

    // �ӳ�ִ�е� Coroutine
    private IEnumerator DelayedAction()
    {
        // �ȴ� 0.2 ��
        yield return new WaitForSeconds(0.2f);

        // ִ�к����߼�
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        // ͬʱ�������� A �� MeshRenderer
        if (objectA != null)
        {
            MeshRenderer objectAMeshRenderer = objectA.GetComponent<MeshRenderer>();
            if (objectAMeshRenderer != null)
            {
                objectAMeshRenderer.enabled = true;
            }
        }
    }

    // �¼�����ʱ���õĴ�����
    private void OnWhiteEventTriggered()
    {
        // ��������Դ����¼���������߼�
        Debug.Log("White event triggered!");
    }

    // �ǵ�������ʱȡ�������¼�
    private void OnDestroy()
    {
        EventCenter.Instance.Unsubscribe("white", OnWhiteEventTriggered);
    }
}
