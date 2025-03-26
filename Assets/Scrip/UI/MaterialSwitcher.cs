using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialSwitcher : MonoBehaviour
{
    [Header("Ŀ����������")]
    [Tooltip("��Ҫ����MeshRenderer")]
    public Renderer targetRenderer;  // ��Ҫ��קָ��Ŀ�������MeshRenderer

    [Header("��������")]
    [Tooltip("Ҫ�л����²���")]
    public Material newMaterial;     // ��Ҫ��קָ���²���

    private Renderer myRenderer;
    private bool hasSwitched = false; // �Ƿ��Ѿ��л�������
    private bool targetHasBeenEnabled = false; // ��¼Ŀ���Ƿ����ù�

    void Start()
    {
        // ��ȡ����Renderer���
        myRenderer = GetComponent<Renderer>();

        // ��ȫ��֤
        if (targetRenderer == null)
        {
            Debug.LogWarning("Ŀ������δָ����", this);
            enabled = false;
        }
    }

    void Update()
    {
        // ���Ŀ�����嵱ǰ���ڼ���״̬
        if (targetRenderer.enabled)
        {
            targetHasBeenEnabled = true; // ���Ŀ���������������ù�
        }

        // ����⵽Ŀ�����屻���ù���δִ�й��л�
        if (targetHasBeenEnabled && !hasSwitched)
        {
            SwitchMaterial();
        }
    }

    void SwitchMaterial()
    {
        // �л�����
        if (newMaterial != null)
        {
            myRenderer.material = newMaterial;
            Debug.Log("�������л�", this);

            // ������л�
            hasSwitched = true;

            // ��ѡ�����ýű���������
            // enabled = false;
        }
        else
        {
            Debug.LogWarning("�²���δָ����", this);
        }
    }
}