using UnityEngine;

public class MoveObjectOnEsc : MonoBehaviour
{
    // 引用物体 A 上的 RotateObjectWithMouse 脚本
    public RotateObjectWithMouse rotateObjectWithMouse;

    // 引用物体 B 上的 RightClickEffect 脚本
    public RightClickEffect rightClickEffect;

    // 引用物体 C
    public Transform objectC;

    // 移动的目标Y值（减少12.6 或 增加12.6）
    private float moveAmount = 12.6f;

    // 控制移动速度的曲线
    public AnimationCurve moveCurve;

    // 移动的时间（控制移动过程的时长）
    private float moveTime = 0.5f;  // 你可以根据需要调整这个值

    private bool isMoving = false;  // 判断是否开始移动
    private float elapsedTime = 0f;  // 记录移动经过的时间

    private bool isMovingUp = false;  // 判断物体是减少Y轴还是增加Y轴

    // 新增的 public bool 值
    public bool look = false;  // 初始为 false

    private void Update()
    {
        // 检查物体Y值是否不等于16，更新look变量
        if (transform.position.y != 16f)
        {
            look = true;
        }
        else
        {
            look = false;
        }

        // 检查条件，按下 ESC 键并且满足脚本中的条件
        if (Input.GetKeyDown(KeyCode.Escape) && !isMoving && rotateObjectWithMouse.change && !rightClickEffect.white && !rightClickEffect.isChanging)
        {
            StartCoroutine(MoveObject());
        }
    }

    private System.Collections.IEnumerator MoveObject()
    {
        isMoving = true;
        elapsedTime = 0f;

        Vector3 initialPosition = objectC.position;
        float initialY = initialPosition.y;

        // 根据当前状态确定目标Y值
        float targetPositionY = isMovingUp ? initialY + moveAmount : initialY - moveAmount;

        // 开始沿着 Y 轴移动物体
        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;

            // 计算当前的移动进度
            float t = elapsedTime / moveTime;

            // 使用曲线来控制移动速度
            float curveValue = moveCurve.Evaluate(t);

            // 更新物体 C 的位置
            objectC.position = new Vector3(initialPosition.x, Mathf.Lerp(initialY, targetPositionY, curveValue), initialPosition.z);

            yield return null;
        }

        // 确保物体 C 到达目标位置
        objectC.position = new Vector3(initialPosition.x, targetPositionY, initialPosition.z);

        // 切换移动方向
        isMovingUp = !isMovingUp;

        isMoving = false;  // 移动完成，允许再次操作
    }
}
