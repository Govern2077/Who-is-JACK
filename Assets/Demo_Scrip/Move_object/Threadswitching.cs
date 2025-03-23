using UnityEngine;

public class Threadswitching: MonoBehaviour
{
    [Header("Ŀ�������б�")]
    public GameObject[] targetObjects; // ��Inspector����ק��ֵ

    private MeshRenderer ownRenderer;

    void Start()
    {
        // �����Լ���MeshRenderer���
        ownRenderer = GetComponent<MeshRenderer>();

        // ��ȫУ��
        if (ownRenderer == null)
        {
            Debug.LogError("�Ҳ��������Mesh Renderer�����", this);
            enabled = false;
        }
    }

    void Update()
    {
        bool shouldEnable = false;

        // ��������Ŀ������
        foreach (GameObject target in targetObjects)
        {
            if (target == null || !target.activeInHierarchy) continue;

            MeshRenderer targetRenderer = target.GetComponent<MeshRenderer>();

            // ���Ŀ������û��Mesh Renderer������
            if (targetRenderer == null)
            {
                Debug.LogWarning($"Ŀ������ {target.name} û��Mesh Renderer���", this);
                continue;
            }

            // ��������һ��������Render�ͼ�������
            if (targetRenderer.enabled)
            {
                shouldEnable = true;
                break; // ���ֿ������������������ѭ��
            }
        }

        // ��������Render״̬
        ownRenderer.enabled = shouldEnable;
    }

    // �༭���еİ�ȫУ��
    void OnValidate()
    {
        if (targetObjects == null || targetObjects.Length == 0)
        {
            Debug.LogWarning("���������һ��Ŀ�����嵽�б��У�", this);
        }
    }
}