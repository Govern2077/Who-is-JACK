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
            // 订阅Typewriter的onMessage事件
            typewriter.onMessage.AddListener(OnMessage);

            // 初始化对话索引和状态
            dialogueIndex = 0;
            CurrentLineShown = false;

            // 显示第一段对话
            //typewriter.ShowText(dialoguesLines[dialogueIndex]);
            //也就是这段可以控制显示对话
        }

        void OnDestroy()
        {
            // 取消订阅onMessage事件
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

                 case "changemotion": // 处理动作切换事件
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
        [SerializeField] Animator[] animatorControllers; // Animator Controller列表
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
            Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x - 30f, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z); // 仅减少x轴的旋转

            float t = 0;
            const float duration = 0.5f;

            while (t <= duration)
            {
                t += Time.unscaledDeltaTime;
                float pct = t / duration;

                // 使用SmoothStep来实现缓入缓出的效果
                pct = Mathf.SmoothStep(0f, 1f, pct);

                // 通过Lerp来平滑更新位置
                crate.position = Vector3.Lerp(initialPosition, targetPosition, pct);

                // 通过Lerp插值旋转
                crate.rotation = Quaternion.Lerp(initialRotation, targetRotation, pct);

                yield return null;
            }

            // 确保物体完全到达目标位置和旋转
            crate.position = targetPosition;
            crate.rotation = targetRotation;
        }

        void HandleChangeMotionEvent(string[] parameters)
        {
            // 参数检查
            if (parameters.Length < 2)
            {
                Debug.LogWarning($"<changemotion> 需要两个参数，当前参数数量：{parameters.Length}");
                return;
            }

            // 解析参数 x (Animator索引) 和 y (参数值)
            if (!TryGetInt(parameters[0], out int animatorIndex) || !TryGetInt(parameters[1], out int paramValue))
            {
                Debug.LogWarning($"<changemotion> 参数格式错误，应为整数");
                return;
            }

            // 检查Animator索引是否合法
            if (animatorIndex < 0 || animatorIndex >= animatorControllers.Length)
            {
                Debug.LogWarning($"Animator索引 {animatorIndex} 超出范围（0-{animatorControllers.Length - 1}）");
                return;
            }

            // 获取目标Animator
            Animator targetAnimator = animatorControllers[animatorIndex];
            if (targetAnimator == null)
            {
                Debug.LogWarning($"Animator索引 {animatorIndex} 未分配或为空");
                return;
            }

            // 检查参数 "one Int" 是否存在
            if (!HasParameter("oneInt", targetAnimator))
            {
                Debug.LogWarning($"Animator索引 {animatorIndex} 中未找到参数 'oneInt'");
                return;
            }

            // 设置参数值
            targetAnimator.SetInteger("oneInt", paramValue);
            Debug.Log($"已设置Animator索引 {animatorIndex} 的 'oneInt' 参数为 {paramValue}");
        }

        // 检查Animator是否存在指定参数
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
            // 重置对话索引
            dialogueIndex = 0;

            // 重置显示状态
            CurrentLineShown = false;

            // 重新显示第一段对话
            typewriter.ShowText(dialoguesLines[dialogueIndex]);

            // 如果需要，重置其他状态
            talking = true;
        }
    }
}