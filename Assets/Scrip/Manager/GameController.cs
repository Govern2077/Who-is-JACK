using UnityEngine;

public class GameController: MonoBehaviour
{
    // ͨ��Inspector��ק��ֵ��������
    [Header("������������")]
    [SerializeField] private GameObject objectA;  // ����A��ԭ����
    [SerializeField] private GameObject objectB;  // ����B
    [SerializeField] private GameObject objectC;  // ����C
    public bool ISTALKING;
    public bool ISLOOKING;
    public bool ISROTATING;
    public bool ISCHANGING;


    // �ű��������
    private RightClickEffect rightClickEffect;    // ����A�Ľű�
    private ObjectAnimator objectAnimator;        // ����B�ĵ�һ���ű�
    private RotateObjectWithMouse rotateObject;   // ����B�ĵڶ����ű�
    private MoveObjectOnEsc moveObjectOnEsc;      // ����C�Ľű�

    void Start()
    {
        // ��ʼ������A�����ԭ���ܣ�
        InitializeObjectA();

        // ��ʼ������B���������
        InitializeObjectB();

        // ��ʼ������C���
        InitializeObjectC();
    }

    void InitializeObjectA()
    {
        if (objectA != null)
        {
            rightClickEffect = objectA.GetComponent<RightClickEffect>();
            if (rightClickEffect == null) Debug.LogError("����Aȱ��RightClickEffect�ű�");
        }
        else Debug.LogError("����ק����A��GameController");
    }

    void InitializeObjectB()
    {
        if (objectB != null)
        {
            // ��ȡ����B�ĵ�һ���ű�
            objectAnimator = objectB.GetComponent<ObjectAnimator>();
            if (objectAnimator == null) Debug.LogError("����Bȱ��ObjectAnimator�ű�");

            // ��ȡ����B�ĵڶ����ű�
            rotateObject = objectB.GetComponent<RotateObjectWithMouse>();
            if (rotateObject == null) Debug.LogError("����Bȱ��RotateObjectWithMouse�ű�");
        }
        else Debug.LogError("����ק����B��GameController");
    }

    void InitializeObjectC()
    {
        if (objectC != null)
        {
            moveObjectOnEsc = objectC.GetComponent<MoveObjectOnEsc>();
            if (moveObjectOnEsc == null) Debug.LogError("����Cȱ��MoveObjectOnEsc�ű�");
        }
        else Debug.LogError("����ק����C��GameController");
    }

    void Update()
    {
        // ��ȡ����A�Ĳ�����ԭ���ܣ�
        if (rightClickEffect != null)
        {
            bool isChanging = rightClickEffect.isChanging;
            bool isWhite = rightClickEffect.white;
        }

        // ��ȡ����B�Ĳ���
        if (objectAnimator != null && rotateObject != null)
        {
            bool hasClicked = objectAnimator.hasClicked;
            bool canChange = rotateObject.change;
        }

        // ��ȡ����C�Ĳ���
        if (moveObjectOnEsc != null)
        {
            bool isLooking = moveObjectOnEsc.look;
        }
    }
}