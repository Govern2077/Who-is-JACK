using System.Collections;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace Febucci.UI.Examples
{
    [AddComponentMenu(""), DisallowMultipleComponent]
    class ExampleEvents : MonoBehaviour
    {
        // ---- PART OF THE SCRIPT THAT YOU'RE PROBABLY INTERESTED IT ----

        void Start()
        {
            //Subscribe to the event
            typewriter.onMessage.AddListener(OnMessage);


            dialogueIndex = 0;
            CurrentLineShown = false;
            typewriter.ShowText(dialoguesLines[dialogueIndex]);
        }

        void OnDestroy()
        {
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
    }
}