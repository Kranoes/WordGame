using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
namespace GuessWordGame
{
    public class Cell : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI letterText;
        [SerializeField] private Image cellImage;

        [Header("Colors")]
        [SerializeField] private Color defaultColor = new Color32(60, 60, 62, 255);     // #3C3C3E — тёмно-серый фон пустой клетки
        [SerializeField] private Color greenColor = new Color32(106, 170, 100, 255);   // #6AAA64 — мягкий зелёный
        [SerializeField] private Color yellowColor = new Color32(201, 180, 88, 255);   // #C9B458 — мягкий жёлтый
        [SerializeField] private Color grayColor = new Color32(120, 124, 126, 255);   // #787C7E — светлый серый (чётко виден!)
        [SerializeField] private Color highlightColor = Color.white;

        private Color baseColor;

        private void Awake()
        {
            FindComponents();
            baseColor = defaultColor;
        }

        private void FindComponents()
        {
            if (cellImage == null) cellImage = GetComponent<Image>();
            if (letterText == null) letterText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Initialize(int row, int col)
        {
            FindComponents();
            ResetToDefault();
        }

        public void SetLetter(char letter)
        {
            if (letterText == null) return;

            if (letter == ' ' || letter == '\0')
            {
                letterText.text = "";
            }
            else
            {
                letterText.text = char.ToUpper(letter).ToString();
            }
        }

        public string GetLetter()
        {
            return letterText != null ? letterText.text.Trim() : "";
        }
        // 1. Плавный переворот ячейки (медленнее на 30%)
        public IEnumerator AnimateFlipRoutine(LetterState state, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            float duration = 0.19f; // Было 0.16f
            float time = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 compressedScale = new Vector3(1f, 0f, 1f);

            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, compressedScale, time / duration);
                yield return null;
            }

            SetState(state);

            time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localScale = Vector3.Lerp(compressedScale, startScale, time / duration);
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        public IEnumerator AnimateShakeRoutine()
        {
            Vector3 originalPosition = transform.localPosition;
            float duration = 0.52f; // Было 0.45f
            float elapsed = 0f;
            float strength = 8f;

            while (elapsed < duration)
            {
                float offsetX = Mathf.Sin(elapsed * 35f) * strength;
                transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }
        public void SetState(LetterState state)
        {
            switch (state)
            {
                case LetterState.Green: baseColor = greenColor; break;
                case LetterState.Yellow: baseColor = yellowColor; break;
                case LetterState.Gray: baseColor = grayColor; break;
                default: baseColor = defaultColor; break;
            }

            if (cellImage != null) cellImage.color = baseColor;
        }

        public void Highlight(bool isHighlighted)
        {
            if (cellImage == null) return;
            cellImage.color = isHighlighted ? highlightColor : baseColor;
        }

        public void ResetToDefault()
        {
            SetLetter(' ');
            baseColor = defaultColor;
            if (cellImage != null) cellImage.color = baseColor;
            Highlight(false);
        }
    }
}