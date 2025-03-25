using System.Collections;
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

                 case "changemotion": // �������л��¼�
                    HandleChangeMotionEvent(eventData.parameters);
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
        [SerializeField] Animator[] animatorControllers; // Animator Controller�б�
        Vector3[] cratesInitialScale;

        int dialogueIndex = 0;
        int dialogueLength;
        bool currentLineShown;

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
            if (Input.anyKeyDown && CurrentLineShown)
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
            Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x - 30f, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z); // ������x�����ת

            float t = 0;
            const float duration = 0.5f;

            while (t <= duration)
            {
                t += Time.unscaledDeltaTime;
                float pct = t / duration;

                // ʹ��SmoothStep��ʵ�ֻ��뻺����Ч��
                pct = Mathf.SmoothStep(0f, 1f, pct);

                // ͨ��Lerp��ƽ������λ��
                crate.position = Vector3.Lerp(initialPosition, targetPosition, pct);

                // ͨ��Lerp��ֵ��ת
                crate.rotation = Quaternion.Lerp(initialRotation, targetRotation, pct);

                yield return null;
            }

            // ȷ��������ȫ����Ŀ��λ�ú���ת
            crate.position = targetPosition;
            crate.rotation = targetRotation;
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