using UnityEngine;

public class MeshRendererControl : MonoBehaviour
{
    public GameObject objectA; // ������ק����A
    public GameObject objectC; // ������ק����C
    private MeshRenderer meshRenderer; // ��ǰ����� MeshRenderer
    private RotateObjectWithMouse rotateScript; // ����A�ϵ� RotateObjectWithMouse �ű�
    private MeshRenderer meshRendererC; // ����C�� MeshRenderer

    void Start()
    {
        // ��ȡ��ǰ����� MeshRenderer
        meshRenderer = GetComponent<MeshRenderer>();

        // ��ȡ����A�ϵ� RotateObjectWithMouse �ű�
        if (objectA != null)
        {
            rotateScript = objectA.GetComponent<RotateObjectWithMouse>();
        }

        // ��ȡ����C�� MeshRenderer
        if (objectC != null)
        {
            meshRendererC = objectC.GetComponent<MeshRenderer>();
        }

        // ��ʼ��ʱ������������ MeshRenderer ��״̬
        UpdateMeshRendererStatus();
    }

    void Update()
    {
        // ÿ֡��鲢���� MeshRenderer ״̬
        UpdateMeshRendererStatus();
    }

    void UpdateMeshRendererStatus()
    {
        if (meshRendererC != null && rotateScript != null)
        {
            // �������C�� MeshRenderer �򿪣����� RotateObjectWithMouse �ű��е� change Ϊ true
            if (meshRendererC.enabled && rotateScript.change)
            {
                // ���õ�ǰ����� MeshRenderer
                meshRenderer.enabled = true;
            }
            else
            {
                // ����رյ�ǰ����� MeshRenderer
                meshRenderer.enabled = false;
            }
        }
        else
        {
            // ����Ҳ����ű�������C�� MeshRenderer����رյ�ǰ����� MeshRenderer
            meshRenderer.enabled = false;
        }
    }
}
