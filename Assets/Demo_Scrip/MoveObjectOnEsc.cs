using UnityEngine;

public class MoveObjectOnEsc : MonoBehaviour
{
    // �������� A �ϵ� RotateObjectWithMouse �ű�
    public RotateObjectWithMouse rotateObjectWithMouse;

    // �������� B �ϵ� RightClickEffect �ű�
    public RightClickEffect rightClickEffect;

    // �������� C
    public Transform objectC;

    // �ƶ���Ŀ��Yֵ������12.6 �� ����12.6��
    private float moveAmount = 12.6f;

    // �����ƶ��ٶȵ�����
    public AnimationCurve moveCurve;

    // �ƶ���ʱ�䣨�����ƶ����̵�ʱ����
    private float moveTime = 0.5f;  // ����Ը�����Ҫ�������ֵ

    private bool isMoving = false;  // �ж��Ƿ�ʼ�ƶ�
    private float elapsedTime = 0f;  // ��¼�ƶ�������ʱ��

    private bool isMovingUp = false;  // �ж������Ǽ���Y�ỹ������Y��

    // ������ public bool ֵ
    public bool look = false;  // ��ʼΪ false

    private void Update()
    {
        // �������Yֵ�Ƿ񲻵���16������look����
        if (transform.position.y != 16f)
        {
            look = true;
        }
        else
        {
            look = false;
        }

        // ������������� ESC ����������ű��е�����
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

        // ���ݵ�ǰ״̬ȷ��Ŀ��Yֵ
        float targetPositionY = isMovingUp ? initialY + moveAmount : initialY - moveAmount;

        // ��ʼ���� Y ���ƶ�����
        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;

            // ���㵱ǰ���ƶ�����
            float t = elapsedTime / moveTime;

            // ʹ�������������ƶ��ٶ�
            float curveValue = moveCurve.Evaluate(t);

            // �������� C ��λ��
            objectC.position = new Vector3(initialPosition.x, Mathf.Lerp(initialY, targetPositionY, curveValue), initialPosition.z);

            yield return null;
        }

        // ȷ������ C ����Ŀ��λ��
        objectC.position = new Vector3(initialPosition.x, targetPositionY, initialPosition.z);

        // �л��ƶ�����
        isMovingUp = !isMovingUp;

        isMoving = false;  // �ƶ���ɣ������ٴβ���
    }
}
