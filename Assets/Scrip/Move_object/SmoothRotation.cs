using UnityEngine;
using System.Collections;

public class SmoothRotation : MonoBehaviour
{
    [Header("Alignment Settings")]
    public Vector3 alignmentVector = Vector3.back;  // 本地坐标系对齐方向（可在Inspector修改）
    public float maxAngle = 2f;                     // 触发对齐的角度阈值（度）
    public float rotationDuration = 0.2f;           // 对齐持续时间（秒）

    [Header("Visualization")]
    public Color objectVectorColor = Color.blue;    // 物体向量颜色
    public Color targetVectorColor = Color.red;     // 目标向量颜色
    public float vectorLength = 1f;                 // 可视化向量长度

    [Header("References")]
    public GameObject objectA;
    public RightClickEffect rightClickEffect;
    public GameObject objectC;

    private MeshRenderer meshRenderer;
    private Quaternion targetRotation;
    private bool isRotating;
    private float rotationStartTime;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        EventCenter.Instance.Subscribe("white", OnWhiteEventTriggered);
    }

    private void Update()
    {
        if (meshRenderer != null && !meshRenderer.enabled) return;

        // 获取物体C的本地对齐向量（转换为世界坐标系）
        Vector3 worldAlignment = objectC.transform.TransformDirection(alignmentVector.normalized);

        // 计算与目标向量（0,0,-1）的夹角
        float angle = Vector3.Angle(worldAlignment, Vector3.back);

        // 当角度小于阈值且满足条件时开始对齐
        if (angle <= maxAngle &&
            rightClickEffect != null &&
            rightClickEffect.white &&
            !rightClickEffect.isChanging &&
            !isRotating&& !Input.GetMouseButton(0))
        {
            StartRotationAlignment(worldAlignment);
        }

        // 执行旋转插值
        if (isRotating)
        {
            float progress = (Time.time - rotationStartTime) / rotationDuration;
            objectC.transform.rotation = Quaternion.Slerp(
                objectC.transform.rotation,
                targetRotation,
                Mathf.Clamp01(progress)
            );

            // 旋转完成检测（角度小于0.1度）
            if (progress >= 0.999f)
            {
                isRotating = false;
                OnAlignmentComplete();
            }
        }
    }

    // 初始化旋转对齐
    private void StartRotationAlignment(Vector3 currentDirection)
    {
        // 计算从当前方向到目标方向的旋转
        targetRotation = Quaternion.FromToRotation(currentDirection, Vector3.back) * objectC.transform.rotation;
        isRotating = true;
        rotationStartTime = Time.time;
    }

    // 对齐完成处理
    private void OnAlignmentComplete()
    {
        EventCenter.Instance.TriggerEvent("white");
        StartCoroutine(DelayedAction());
    }

    // 可视化向量
    private void OnDrawGizmos()
    {
        if (objectC == null) return;

        // 绘制物体对齐向量（蓝色）
        Gizmos.color = objectVectorColor;
        Vector3 worldStart = objectC.transform.position;
        Vector3 worldAlignment = objectC.transform.TransformDirection(alignmentVector.normalized);
        Gizmos.DrawLine(worldStart, worldStart + worldAlignment * vectorLength);

        // 绘制目标向量（红色）
        Gizmos.color = targetVectorColor;
        Gizmos.DrawLine(worldStart, worldStart + Vector3.back * vectorLength);
    }

    private IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(0.03f);

        if (meshRenderer != null) meshRenderer.enabled = false;

        if (objectA != null)
        {
            MeshRenderer aRenderer = objectA.GetComponent<MeshRenderer>();
            if (aRenderer != null) aRenderer.enabled = true;
        }
    }

    private void OnWhiteEventTriggered()
    {
        Debug.Log("向量对齐完成！");
    }

    private void OnDestroy()
    {
        EventCenter.Instance.Unsubscribe("white", OnWhiteEventTriggered);
    }
}