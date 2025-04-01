using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    [System.Serializable]
    public class PointPair
    {
        public int index;
        public GameObject startObj;
        public GameObject endObj;
    }

    [Header("����������")]
    public List<PointPair> positionPairs = new List<PointPair>();

    [Header("Ԥ����")]
    public GameObject pinPrefab;
    public GameObject linePrefab;

    [Header("��������")]
    public float lineDuration = 2f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public void GenerateLine(int targetIndex)
    {
        PointPair pair = positionPairs.Find(p => p.index == targetIndex);
        if (pair == null)
        {
            Debug.LogWarning($"δ�ҵ�����Ϊ {targetIndex} ��������");
            return;
        }

        if (pair.startObj == null || pair.endObj == null)
        {
            Debug.LogWarning($"���� {targetIndex} ����������δ����");
            return;
        }

        Vector3 startPos = pair.startObj.transform.position;
        Vector3 endPos = pair.endObj.transform.position;

        // ����ͼ������Ϊ������
        GameObject startPin = Instantiate(
        pinPrefab,
        startPos,
        Quaternion.Euler(-90, 0, 0), // �ؼ��޸ĵ�
        transform
    );

        GameObject endPin = Instantiate(
            pinPrefab,
            endPos,
            Quaternion.Euler(-90, 0, 0),
            transform
        );



        // �����߶β���Ϊ������
        GameObject lineObj = Instantiate(linePrefab, startPos, Quaternion.identity, transform);
        Renderer lineRenderer = lineObj.GetComponentInChildren<Renderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("�߶�Ԥ�������ô���");
            Destroy(lineObj);
            return;
        }

        StartCoroutine(AnimateLine(lineObj.transform, lineRenderer, startPos, endPos));
    }

    IEnumerator AnimateLine(Transform line, Renderer lineRenderer, Vector3 start, Vector3 end)
    {
        // ����ԭ�ж����߼�����
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        if (distance < 0.01f) yield break;

        Vector3 normalizedDirection = direction.normalized;
        line.rotation = Quaternion.LookRotation(normalizedDirection);
        line.localScale = new Vector3(0, 1, 1);

        float originalLength = lineRenderer.bounds.size.x;
        float targetScaleX = distance / originalLength;

        float timer = 0f;
        while (timer < lineDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / lineDuration);
            float curvedProgress = scaleCurve.Evaluate(progress);

            line.localScale = new Vector3(
                Mathf.Lerp(0, targetScaleX, curvedProgress),
                1,
                1
            );

            // �����е�ʱʹ����������
            Vector3 worldMidPoint = Vector3.Lerp(start, end, curvedProgress * 0.5f);
            line.position = worldMidPoint;

            yield return null;
        }

        line.localScale = new Vector3(targetScaleX, 1, 1);
        line.position = (start + end) / 2f;
    }
}