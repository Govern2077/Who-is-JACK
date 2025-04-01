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

                case "Changemotion": // 处理动作切换事件
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
        [SerializeField] Animator[] animatorControllers; // Animator Controller列表
        Vector3[] cratesInitialScale;
        Vector3[] givebacksInitialScale;
        [SerializeField] List<GameObject> triggerObjects; // 新增trigger物体列表

        [Header("对话流程控制")]
        public bool canContinue = true; // 控制是否允许继续对话

        [Header("摄像机震动")]
        [SerializeField] Transform cameraShakeTarget; // 需要震动的摄像机或父物体
        [SerializeField] float shakeDuration = 0.8f;  // 震动总时长
        [SerializeField] float shakeAngle = -0.3f;    // 最大旋转角度

        [System.Serializable]
        public class PointPair
        {
            public List<GameObject> Obj;
        }

        [Header("侦探版动画")]
        public List<PointPair> positionPairs = new List<PointPair>();

        int dialogueIndex = 0;
        int dialogueLength;
        bool currentLineShown;
        bool isShaking = false; // 防止重复震动

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

                // 使用平方运算实现缓出效果（开始快，结尾慢）
                float easedProgress = 1 - Mathf.Pow(1 - linearProgress, 2);

                // 仅对Y轴应用缓出效果，其他轴保持线性
                Vector3 currentPosition = Vector3.Lerp(
                    initialPosition,
                    targetPosition,
                    linearProgress // X和Z轴保持线性
                );
                currentPosition.y = Mathf.Lerp( // 单独处理Y轴的缓出
                    initialPosition.y,
                    targetPosition.y,
                    easedProgress
                );

                crate.position = currentPosition;

                // 旋转使用完整的缓出效果
                crate.rotation = Quaternion.Lerp(
                    initialRotation,
                    targetRotation,
                    easedProgress
                );

                yield return null;
            }

            // 确保最终状态精确
            crate.position = targetPosition;
            crate.rotation = targetRotation;
        }

        IEnumerator AnimateGive(int givebackIndex)
        {
            Transform giveback = givebacks[givebackIndex];
            Vector3 initialPosition = giveback.position;
            Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y + 7, initialPosition.z); // 改为+Y方向

            Quaternion initialRotation = giveback.rotation;
            Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x + 30f, // 改为+X轴旋转
                                                        initialRotation.eulerAngles.y,
                                                        initialRotation.eulerAngles.z);

            float t = 0;
            const float duration = 0.5f;

            while (t <= duration)
            {
                t += Time.unscaledDeltaTime;
                float linearProgress = t / duration;

                // 使用平方运算实现缓入效果（开始慢，结尾快）
                float easedProgress = Mathf.Pow(linearProgress, 2); // 修改为平方缓入

                // 仅对Y轴应用缓入效果，其他轴保持线性
                Vector3 currentPosition = Vector3.Lerp(
                    initialPosition,
                    targetPosition,
                    linearProgress // X和Z轴保持线性
                );
                currentPosition.y = Mathf.Lerp( // 单独处理Y轴的缓入
                    initialPosition.y,
                    targetPosition.y,
                    easedProgress
                );

                giveback.position = currentPosition;

                // 旋转使用完整的缓入效果
                giveback.rotation = Quaternion.Lerp(
                    initialRotation,
                    targetRotation,
                    easedProgress
                );

                yield return null;
            }

            // 确保最终状态精确
            giveback.position = targetPosition;
            giveback.rotation = targetRotation;
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

        void HandleTriggerEvent1(string[] parameters, bool activate)
        {
            // 参数检查
            if (parameters.Length == 0)
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> 需要指定物体索引");
                return;
            }

            // 解析索引
            if (!TryGetInt(parameters[0], out int index))
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> 参数必须为整数");
                return;
            }

            // 索引有效性检查
            if (index < 0 || index >= triggerObjects.Count)
            {
                Debug.LogWarning($"Trigger索引 {index} 超出范围（0-{triggerObjects.Count - 1}）");
                return;
            }

            // 获取目标物体
            GameObject target = triggerObjects[index];
            if (target == null)
            {
                Debug.LogWarning($"Trigger索引 {index} 的物体未分配");
                return;
            }

            // 设置激活状态
            target.SetActive(activate);
            Debug.Log($"{(activate ? "激活" : "禁用")}了Trigger物体：{target.name}");
        }

        void HandleTriggerEvent2(string[] parameters, bool activate)
        {
            // 参数检查
            if (parameters.Length == 0)
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> 需要指定物体索引");
                return;
            }

            // 解析索引
            if (!TryGetInt(parameters[0], out int index))
            {
                Debug.LogWarning($"<{(activate ? "up" : "down")}> 参数必须为整数");
                return;
            }

            // 索引有效性检查
            if (index < 0 || index >= triggerObjects.Count)
            {
                Debug.LogWarning($"Trigger索引 {index} 超出范围（0-{triggerObjects.Count - 1}）");
                return;
            }

            // 获取目标物体
            GameObject target = triggerObjects[index];
            if (target == null)
            {
                Debug.LogWarning($"Trigger索引 {index} 的物体未分配");
                return;
            }

            // 设置激活状态
            target.SetActive(false);
            Debug.Log($"{(activate ? "激活" : "禁用")}了Trigger物体：{target.name}");
        }

        void HandleCameraShake(string[] parameters)
        {
            if (isShaking) return;

            // 参数验证
            if (cameraShakeTarget == null)
            {
                Debug.LogWarning("未指定摄像机震动目标物体");
                return;
            }

            // 可选参数：自定义震动强度
            if (parameters.Length > 0 && TryGetFloat(parameters[0], out float customAngle))
            {
                shakeAngle = customAngle;
            }

            StartCoroutine(ShakeCamera());
        }

        // 添加TryGetFloat方法（如果不存在）
        bool TryGetFloat(string parameter, out float result)
        {
            return FormatUtils.TryGetFloat(parameter, 0, out result);
        }

        // 摄像机震动协程
        IEnumerator ShakeCamera()
        {
            isShaking = true;
            Quaternion originalRot = cameraShakeTarget.localRotation;

            // 明确震动次数（3次完整来回）
            const int totalShakes = 2;
            float singleShakeTime = shakeDuration / totalShakes;

            for (int i = 0; i < totalShakes; i++)
            {
                float shakeTimer = 0f;

                while (shakeTimer < singleShakeTime)
                {
                    shakeTimer += Time.deltaTime;
                    float progress = Mathf.Clamp01(shakeTimer / singleShakeTime);

                    // 使用正弦波实现平滑震动
                    float angle = Mathf.Sin(progress * Mathf.PI * 2) * shakeAngle;
                    cameraShakeTarget.localRotation = originalRot * Quaternion.Euler(angle, 0, 0);

                    yield return null;
                }
            }

            // 恢复原始旋转
            cameraShakeTarget.localRotation = originalRot;
            isShaking = false;
        }

        // 平滑曲线函数（三次缓动）
        float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
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