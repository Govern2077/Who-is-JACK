using UnityEngine;

public class GameController: MonoBehaviour
{
    // 通过Inspector拖拽赋值所有物体
    [Header("物体引用配置")]
    [SerializeField] private GameObject objectA;  // 物体A（原需求）
    [SerializeField] private GameObject objectB;  // 物体B
    [SerializeField] private GameObject objectC;  // 物体C
    public bool ISTALKING;
    public bool ISLOOKING;
    public bool ISROTATING;
    public bool ISCHANGING;


    // 脚本组件缓存
    private RightClickEffect rightClickEffect;    // 物体A的脚本
    private ObjectAnimator objectAnimator;        // 物体B的第一个脚本
    private RotateObjectWithMouse rotateObject;   // 物体B的第二个脚本
    private MoveObjectOnEsc moveObjectOnEsc;      // 物体C的脚本

    void Start()
    {
        // 初始化物体A组件（原功能）
        InitializeObjectA();

        // 初始化物体B的两个组件
        InitializeObjectB();

        // 初始化物体C组件
        InitializeObjectC();
    }

    void InitializeObjectA()
    {
        if (objectA != null)
        {
            rightClickEffect = objectA.GetComponent<RightClickEffect>();
            if (rightClickEffect == null) Debug.LogError("物体A缺少RightClickEffect脚本");
        }
        else Debug.LogError("请拖拽物体A到GameController");
    }

    void InitializeObjectB()
    {
        if (objectB != null)
        {
            // 获取物体B的第一个脚本
            objectAnimator = objectB.GetComponent<ObjectAnimator>();
            if (objectAnimator == null) Debug.LogError("物体B缺少ObjectAnimator脚本");

            // 获取物体B的第二个脚本
            rotateObject = objectB.GetComponent<RotateObjectWithMouse>();
            if (rotateObject == null) Debug.LogError("物体B缺少RotateObjectWithMouse脚本");
        }
        else Debug.LogError("请拖拽物体B到GameController");
    }

    void InitializeObjectC()
    {
        if (objectC != null)
        {
            moveObjectOnEsc = objectC.GetComponent<MoveObjectOnEsc>();
            if (moveObjectOnEsc == null) Debug.LogError("物体C缺少MoveObjectOnEsc脚本");
        }
        else Debug.LogError("请拖拽物体C到GameController");
    }

    void Update()
    {
        // 获取物体A的参数（原功能）
        if (rightClickEffect != null)
        {
            bool isChanging = rightClickEffect.isChanging;
            bool isWhite = rightClickEffect.white;
        }

        // 获取物体B的参数
        if (objectAnimator != null && rotateObject != null)
        {
            bool hasClicked = objectAnimator.hasClicked;
            bool canChange = rotateObject.change;
        }

        // 获取物体C的参数
        if (moveObjectOnEsc != null)
        {
            bool isLooking = moveObjectOnEsc.look;
        }
    }
}