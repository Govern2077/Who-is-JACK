using UnityEngine;

public class RotateObjectWithMouse : MonoBehaviour
{
    // 将物体A声明为public变量
    public GameObject objectA;  // 物体A

    // 公共的布尔变量，用于控制是否可以旋转
    public bool change = false;  // 控制旋转的开关

    private Vector3 lastMousePosition;
    private bool isMousePressed = false;

    // 用于记录旋转速度
    private Vector3 rotationVelocity = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;

    // 用于控制衰减时间和旋转速度
    public float decelerationTime = 0.5f;  // 旋转减速时间

    private void OnEnable()
    {
        // 订阅事件
        EventCenter.Instance.Subscribe("change", OnChangeEventTriggered);
    }

    private void OnDisable()
    {
        // 取消订阅事件
        EventCenter.Instance.Unsubscribe("change", OnChangeEventTriggered);
    }

    private void OnChangeEventTriggered()
    {
        // 当事件触发时，将change设置为true
        change = true;
        Debug.Log("旋转已启用");
    }

    void Update()
    {
        // 每帧调用 UpdateRotation 方法
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // 如果change为false，不执行任何旋转操作
        if (!change)
        {
            return;
        }

        // 检测鼠标左键是否按下
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            lastMousePosition = Input.mousePosition; // 记录按下时的鼠标位置
            Debug.Log("can move");
        }

        // 检测鼠标左键是否松开
        if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            // 在松开鼠标时记录旋转速度
            rotationVelocity = currentVelocity;
        }

        // 如果鼠标按下，计算旋转
        if (isMousePressed && objectA != null)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition; // 计算鼠标的位移
            lastMousePosition = Input.mousePosition; // 更新鼠标位置

            // 将鼠标位移转换为旋转角度
            float rotateSpeed = 0.1f; // 旋转速度系数
            float rotateX = mouseDelta.y * rotateSpeed;
            float rotateY = -mouseDelta.x * rotateSpeed;

            // 更新当前的旋转速度
            currentVelocity = new Vector3(rotateX, rotateY, 0);

            // 旋转物体A
            objectA.transform.Rotate(Vector3.up, rotateY, Space.World); // 绕Y轴旋转
            objectA.transform.Rotate(Vector3.right, rotateX, Space.World); // 绕X轴旋转
        }

        // 如果鼠标松开，旋转继续减速
        if (!isMousePressed && rotationVelocity.magnitude > 0)
        {
            // 逐渐减少旋转速度
            float decelerationFactor = Time.deltaTime / decelerationTime;
            rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.zero, decelerationFactor);

            // 根据旋转速度继续旋转
            objectA.transform.Rotate(Vector3.up, rotationVelocity.y, Space.World);
            objectA.transform.Rotate(Vector3.right, rotationVelocity.x, Space.World);
        }
    }
}
