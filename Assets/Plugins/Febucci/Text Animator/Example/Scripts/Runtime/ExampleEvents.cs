using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace Febucci.UI.Examples
{
    [AddComponentMenu(""), DisallowMultipleComponent]
    public class ExampleEvents : MonoBehaviour
    {
        // ---- PART OF THE SCRIPT THAT YOU'RE PROBABLY INTERESTED IT ----

        void Start()
        {
            // ����Typewriter��onMessage�¼�
            typewriter.onMessage.AddListener(OnMessage);

            // ��ʼ���Ի�������״̬
            dialogueIndex = 0;
            CurrentLineShown = false;

            // ��ʾ��һ�ζԻ�
            //typewriter.ShowText(dialoguesLines[dialogueIndex]);
            //Ҳ������ο��Կ�����ʾ�Ի�
        }

        void OnDestroy()
        {
            // ȡ������onMessage�¼�
            if (typewriter) typewriter.onMessage.RemoveListener(OnMessage);
        }

        bool TryGetInt(string parameter, out int result)
        {

            if (FormatUtils.TryGetFloat(parameter, 0, out float resultFloat))
            {
                result = (int)resultFloat;
                return true;
            }

            result = -1;
            return false;
        }
        void OnMessage(EventMarker eventData)
        {
            switch (eventData.name)
            {
                case "face":
                    if (eventData.parameters.Length <= 0)
                    {
                        Debug.LogWarning($"You need to specify a sprite index! Dialogue: {dialogueIndex}");
                        return;
                    }

                    if (TryGetInt(eventData.parameters[0], out int spriteIndex))
                    {
                        if (spriteIndex >= 0 && spriteIndex < faces.Length)
                        {
                            faceRenderer.sprite = faces[spriteIndex];
                        }
                        else
                        {
                            Debug.Log($"Sprite index was out of range. Dialogue: {dialogueIndex}");
                        }
                    }
                    break;

                case "crate":
                    if (eventData.parameters.Length <= 0)
                    {
                        Debug.LogWarning($"You need to specify a crate index! Dialogue: {dialogueIndex}");
                        return;
                    }

                    if (TryGetInt(eventData.parameters[0], out int crateIndex))
                    {
                        if (crateIndex >= 0 && crateIndex < crates.Length)
                        {
                            StartCoroutine(AnimateCrate(crateIndex));
                        }
                        else
                        {
                            Debug.Log($"Sprite index was out of range. Dialogue: {dialogueIndex}");
                        }
                    }
                    break;

                case "giveback":
                    if (eventData.parameters.Length <= 0)
                    {
                        Debug.LogWarning($"You need to specify a crate index! Dialogue: {dialogueIndex}");
                        return;
                    }

                    if (TryGetInt(eventData.parameters[0], out int givebackIndex))
                    {
                        if (givebackIndex >= 0 && givebackIndex < givebacks.Length)
                        {
                            StartCoroutine(AnimateGive(givebackIndex));
                        }
                        else
                        {
                            Debug.Log($"Sprite index was out of range. Dialogue: {dialogueIndex}");
                        }
                    }
                    break;

                case "Changemotion": // ��������л��¼�
                    HandleChangeMotionEvent(eventData.parameters);
                    break;

                case "Triggerup":
                    HandleTriggerEvent1(eventData.parameters, true);
                    break;

                case "Triggerdown":
                    HandleTriggerEvent2(eventData.parameters, false);
                    break;

                case "CameraShake":
                    HandleCameraShake(eventData.parameters);
                    break;

            }
        }

        // ---- OTHER PART OF THE SCRIPT ----
        // This makes the script run faking a dialogue system
        [SerializeField] TypewriterCore typewriter;
        [SerializeField, TextArea(1, 5)] string[] dialoguesLines;
        [SerializeField] Sprite[] faces;
        [SerializeField] SpriteRenderer faceRenderer;
        [SerializeField] GameObject continueText;
        [SerializeField] Transform[] crates;
        [SerializeField] Transform[] givebacks;
        [SerializeField] Animator[] animatorControllers; // Animator Controller�б�
        Vector3[] cratesInitialScale;
        Vector3[] givebacksInitialScale;
        [SerializeField] List<GameObject> triggerObjects; // ����trigger�����б�

        [Header("�Ի����̿���")]
        public bool canContinue = true; // �����Ƿ���������Ի�

        [Header("�������")]
        [SerializeField] Transform cameraShakeTarget; // ��Ҫ�𶯵������������
        [SerializeField] float shakeDuration = 0.8f;  // ����ʱ��
        [SerializeField] float shakeAngle = -0.3f;    // �����ת�Ƕ�

        [System.Serializable]
        public class PointPair
        {
            public List<GameObject> Obj;
        }

        [Header("��̽�涯��")]
        public List<PointPair> positionPairs = new List<PointPair>();

        int dialogueIndex = 0;
        int dialogueLength;
        bool currentLineShown;
        bool isShaking = false; // ��ֹ�ظ���

        bool CurrentLineShown
        {
            get => currentLineShown;
            set
            {
                currentLineShown = value;
                continueText.SetActive(value);
            }
        }

        void Awake()
        {
            cratesInitialScale = new Vector3[crates.Length];
            for (int i = 0; i < crates.Length; i++)
            {
                cratesInitialScale[i] = crates[i].localScale;
            }

            dialogueLength = dialoguesLines.Length;
            typewriter.onTextShowed.AddListener(() => CurrentLineShown = true);
        }
        public bool talking = true;
        void ContinueSequence()
        {
            CurrentLineShown = false;
            dialogueIndex++;
            if (dialogueIndex < dialogueLength)
            {
                typewriter.ShowText(dialoguesLines[dialogueIndex]);
            }
            else
            {
                talking = false;
                typewriter.StartDisappearingText();
            }
        }

        void Update()
        {
            if (canContinue && Input.anyKeyDown && CurrentLineShown)
            {
                ContinueSequence();
            }
        }

        IEnumerator AnimateCrate(int crateIndex)
        {
            Transform crate = crates[crateIndex];
            Vector3 initialPosition = crate.position;
            Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y - 7, initialPosition.z);

            Quaternion initialRotation = crate.rotation;
            Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x - 30f, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);

            float t = 0;
            const float duration = 0.5f;

            while (t <= duration)
            {
                t += Time.unscaledDeltaTime;
                float linearProgress = t / duration;

                // ʹ��ƽ������ʵ�ֻ���Ч������ʼ�죬��β����
                float easedProgress = 1 - Mathf.Pow(1 - linearProgress, 2);

                // ����Y��Ӧ�û���Ч���������ᱣ������
                Vector3 currentPosition = Vector3.Lerp(
                    initialPosition,
                    targetPosition,
                    linearProgress // X��Z�ᱣ������
                );
                currentPosition.y = Mathf.Lerp( // ��������Y��Ļ���
                    initialPosition.y,
                    targetPosition.y,
                    easedProgress
                );

                crate.position = currentPosition;

                // ��תʹ�������Ļ���Ч��
                crate.rotation = Quaternion.Lerp(
                    initialRotation,
                    targetRotation,
                    easedProgress
                );

                yield return null;
            }

            // ȷ������״̬��ȷ
            crate.position = targetPosition;
            crate.rotation = targetRotation;
        }

        IEnumerator AnimateGive(int givebackIndex)
        {
            Transform giveback = givebacks[givebackIndex];
            Vector3 initialPosition = giveback.position;
            Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y + 7, initialPosition.z); // ��Ϊ+Y����

            Quaternion initialRotation = giveback.rotation;
            Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x + 30f, // ��Ϊ+X����ת
                                                        initialRotation.eulerAngles.y,
                                                        initialRotation.eulerAngles.z);

            float t = 0;
            const float duration = 0.5f;

            while (t <= duration)
            {
                t += Time.unscaledDeltaTime;
                float linearProgress = t / duration;

                // ʹ��ƽ������ʵ�ֻ���Ч������ʼ������β�죩
                float easedProgress = Mathf.Pow(linearProgress, 2); // �޸�Ϊƽ������

                // ����Y��Ӧ�û���Ч���������ᱣ������
                Vector3 currentPosition = Vector3.Lerp(
                    initialPosition,
                    targetPosition,
                    linearProgress // X��Z�ᱣ������
                );
                currentPosition.y = Mathf.Lerp( // ��������Y��Ļ���
                    initialPosition.y,
                    targetPosition.y,
                    easedProgress
                );

                giveback.position = currentPosition;

                // ��תʹ�������Ļ���Ч��
                giveback.rotation = Quaternion.Lerp(
                    initialRotation,
                    targetRotation,
                    easedProgress
                );

                yield return null;
            }

            // ȷ������״̬��ȷ
            giveback.position = targetPosition;
            giveback.rotation = targetRotation;
        }
        void HandleChangeMotionEvent(string[] parameters)
        {
            // �������
            if (parameters.Length < 2)
            {
                Debug.LogWarning($"<changemotion> ��Ҫ������������ǰ����������{parameters.Length}");
                return;
            }

            // �������� x (Animator����) �� y (����ֵ)
            if (!TryGetInt(parameters[0], out int animatorIndex) || !TryGetInt(parameters[1], out int paramValue))
            {
                Debug.LogWarning($"<changemotion> ������ʽ����ӦΪ����");
                return;
            }

            // ���Animator�����Ƿ�Ϸ�
            if (animatorIndex < 0 || animatorIndex >= animatorControllers.Length)
            {
                Debug.LogWarning($"Animator���� {animatorIndex} ������Χ��0-{animatorControllers.Length - 1}��");
                return;
            }

            // ��ȡĿ��Animator
            Animator targetAnimator = animatorControllers[animatorIndex];
            if (targetAnimator == null)
            {
                Debug.LogWarning($"Animator���� {animatorIndex} δ�����Ϊ��");
                return;
            }

            // ������ "one Int" �Ƿ����
            if (!HasParameter("oneInt", targetAnimator))
            {
                Debug.LogWarning($"Animator���� {animatorIndex} ��δ�ҵ����� 'oneInt'");
                return;
            }

            // ���ò���ֵ
            targetAnimator.SetInteger("oneInt", paramValue);
            Debug.Log($"������Animator���� {animatorIndex} �� 'oneInt' ����Ϊ {paramValue}");
        }

        // ���Animator�Ƿ����ָ������
        bool HasParameter(string paramName, Animator animator)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName && param.type == AnimatorControllerParameterType.Int)
                {
                    return true;
                }
            }
            return false;
        }

        void HandleTriggerEvent1(string[] parameters, bool activate)
        {
            // �������
            if (parameters.Length == 0)
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> ��Ҫָ����������");
                return;
            }

            // ��������
            if (!TryGetInt(parameters[0], out int index))
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> ��������Ϊ����");
                return;
            }

            // ������Ч�Լ��
            if (index < 0 || index >= triggerObjects.Count)
            {
                Debug.LogWarning($"Trigger���� {index} ������Χ��0-{triggerObjects.Count - 1}��");
                return;
            }

            // ��ȡĿ������
            GameObject target = triggerObjects[index];
            if (target == null)
            {
                Debug.LogWarning($"Trigger���� {index} ������δ����");
                return;
            }

            // ���ü���״̬
            target.SetActive(activate);
            Debug.Log($"{(activate ? "����" : "����")}��Trigger���壺{target.name}");
        }

        void HandleTriggerEvent2(string[] parameters, bool activate)
        {
            // �������
            if (parameters.Length == 0)
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> ��Ҫָ����������");
                return;
            }

            // ��������
            if (!TryGetInt(parameters[0], out int index))
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> ��������Ϊ����");
                return;
            }

            // ������Ч�Լ��
            if (index < 0 || index >= triggerObjects.Count)
            {
                Debug.LogWarning($"Trigger���� {index} ������Χ��0-{triggerObjects.Count - 1}��");
                return;
            }

            // ��ȡĿ������
            GameObject target = triggerObjects[index];
            if (target == null)
            {
                Debug.LogWarning($"Trigger���� {index} ������δ����");
                return;
            }

            // ���ü���״̬
            target.SetActive(false);
            Debug.Log($"{(activate ? "����" : "����")}��Trigger���壺{target.name}");
        }

        void HandleCameraShake(string[] parameters)
        {
            if (isShaking) return;

            // ������֤
            if (cameraShakeTarget == null)
            {
                Debug.LogWarning("δָ���������Ŀ������");
                return;
            }

            // ��ѡ�������Զ�����ǿ��
            if (parameters.Length > 0 && TryGetFloat(parameters[0], out float customAngle))
            {
                shakeAngle = customAngle;
            }

            StartCoroutine(ShakeCamera());
        }

        // ���TryGetFloat��������������ڣ�
        bool TryGetFloat(string parameter, out float result)
        {
            return FormatUtils.TryGetFloat(parameter, 0, out result);
        }

        // �������Э��
        IEnumerator ShakeCamera()
        {
            isShaking = true;
            Quaternion originalRot = cameraShakeTarget.localRotation;

            // ��ȷ�𶯴�����3���������أ�
            const int totalShakes = 2;
            float singleShakeTime = shakeDuration / totalShakes;

            for (int i = 0; i < totalShakes; i++)
            {
                float shakeTimer = 0f;

                while (shakeTimer < singleShakeTime)
                {
                    shakeTimer += Time.deltaTime;
                    float progress = Mathf.Clamp01(shakeTimer / singleShakeTime);

                    // ʹ�����Ҳ�ʵ��ƽ����
                    float angle = Mathf.Sin(progress * Mathf.PI * 2) * shakeAngle;
                    cameraShakeTarget.localRotation = originalRot * Quaternion.Euler(angle, 0, 0);

                    yield return null;
                }
            }

            // �ָ�ԭʼ��ת
            cameraShakeTarget.localRotation = originalRot;
            isShaking = false;
        }

        // ƽ�����ߺ��������λ�����
        float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        public void RestartDialogue()
        {
            // ���öԻ�����
            dialogueIndex = 0;

            // ������ʾ״̬
            CurrentLineShown = false;

            // ������ʾ��һ�ζԻ�
            typewriter.ShowText(dialoguesLines[dialogueIndex]);

            // �����Ҫ����������״̬
            talking = true;
        }
    }
}