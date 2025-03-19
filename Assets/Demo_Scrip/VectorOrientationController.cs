using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class VectorOrientationController : MonoBehaviour
{
    [Header("Orientation Settings")]
    public Vector3 specifiedVector = Vector3.forward;
    public bool useLocalSpace = true;
    public GameObject objectC; // 需要旋转的目标物体

    [Header("Rotation Parameters")]
    [Range(88f, 90f)] public float verticalThreshold = 89f;
    public float rotationDuration = 0.5f;

    [Header("Visualization")]
    public float rayLength = 2f;
    public Color normalColor = Color.cyan;
    public Color activeColor = Color.yellow;
    public Color completedColor = Color.green;

    private MeshRenderer meshRenderer;
    private Vector3 currentDirection;
    private bool isRotating;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        UpdateVectorDirection();
        CheckVerticalCondition();
        DrawDebugVisuals();
    }

    void UpdateVectorDirection()
    {
        currentDirection = useLocalSpace ?
            transform.TransformDirection(specifiedVector) :
            specifiedVector;
        currentDirection.Normalize();
    }

    void CheckVerticalCondition()
    {
        if (isRotating) return;

        // 计算与XY平面法线（Z轴）的夹角（网页4的角度计算原理）
        float angle = Vector3.Angle(currentDirection, Vector3.forward);

        // 当角度进入88-90度范围时启动旋转（网页3的条件检测逻辑）
        if (angle >= (90f - verticalThreshold) && angle <= 90f)
        {
            StartCoroutine(RotateToVertical());
        }
    }

    IEnumerator RotateToVertical()
    {
        isRotating = true;
        Quaternion startRot = objectC.transform.rotation;

        // 计算目标旋转（网页4的四元数生成方法）
        Quaternion targetRot = Quaternion.LookRotation(
            Vector3.forward,
            objectC.transform.up
        );

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);

            // 使用球面插值平滑旋转（网页3的插值方法）
            objectC.transform.rotation = Quaternion.Slerp(
                startRot,
                targetRot,
                t
            );
            yield return null;
        }
        isRotating = false;
    }

    void DrawDebugVisuals()
    {
        Color drawColor = normalColor;

        if (isRotating)
        {
            drawColor = activeColor;
        }
        else if (Vector3.Angle(currentDirection, Vector3.forward) < 1f)
        {
            drawColor = completedColor;
        }

        Debug.DrawRay(
            transform.position,
            currentDirection * rayLength,
            drawColor
        );
        DrawArrowhead(transform.position + currentDirection * rayLength, currentDirection, drawColor);
    }

    void DrawArrowhead(Vector3 tipPos, Vector3 direction, Color color)
    {
        float arrowSize = 0.3f;
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized * arrowSize;
        Vector3 up = Vector3.Cross(direction, right).normalized * arrowSize;

        Debug.DrawLine(tipPos, tipPos - direction * arrowSize + right, color);
        Debug.DrawLine(tipPos, tipPos - direction * arrowSize - right, color);
        Debug.DrawLine(tipPos, tipPos - direction * arrowSize + up, color);
        Debug.DrawLine(tipPos, tipPos - direction * arrowSize - up, color);
    }
}