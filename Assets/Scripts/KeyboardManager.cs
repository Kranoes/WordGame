using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GuessWordGame
{
    public class KeyboardManager : MonoBehaviour
    {
        public static KeyboardManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject keyPrefab;
        [SerializeField] private Transform[] rowParents;
        [SerializeField] private Transform specialKeyParent;

        private readonly Dictionary<char, Image> keyImages = new Dictionary<char, Image>();
        private readonly Dictionary<char, LetterState> keyStates = new Dictionary<char, LetterState>();

        private readonly string[] keyboardRows = new string[]
        {
            "ЙЦУКЕНГШЩЗХЪ",
            "ФЫВАПРОЛДЖЭ",
            "ЯЧСМИТЬБЮ"
        };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            CreateKeyboard();
        }

        private void CreateKeyboard()
        {
            keyImages.Clear();
            keyStates.Clear();

            for (int r = 0; r < keyboardRows.Length; r++)
            {
                Transform parent = (r < rowParents.Length && rowParents[r] != null) ? rowParents[r] : transform;

                foreach (char letter in keyboardRows[r])
                {
                    CreateKey(letter, parent);
                }
            }

            if (specialKeyParent != null)
            {
                CreateSpecialKey("<", specialKeyParent, () => GameManager.Instance?.EraseLastLetter());
            }
        }

        public bool DisableRandomIncorrectKeys(string secretWord, int count)
        {
            if (string.IsNullOrEmpty(secretWord)) return false;

            string upperSecret = secretWord.ToUpper();
            List<char> candidates = new List<char>();

            // Ищем буквы, которых нет в секретном слове и которые еще не заблокированы
            foreach (var pair in keyImages)
            {
                char letter = pair.Key;
                if (!upperSecret.Contains(letter) && keyStates[letter] == LetterState.Default)
                {
                    candidates.Add(letter);
                }
            }

            if (candidates.Count == 0) return false;

            int removedCount = 0;
            while (candidates.Count > 0 && removedCount < count)
            {
                int randomIndex = UnityEngine.Random.Range(0, candidates.Count);
                char letterToRemove = candidates[randomIndex];

                // Красим в серый цвет и отключаем кликабельность
                UpdateKeyStatus(letterToRemove, LetterState.Gray);

                if (keyImages.TryGetValue(letterToRemove, out Image img))
                {
                    Button btn = img.GetComponent<Button>();
                    if (btn != null) btn.interactable = false;
                }

                candidates.RemoveAt(randomIndex);
                removedCount++;
            }

            return removedCount > 0;
        }

        private void CreateKey(char letter, Transform parent)
        {
            GameObject keyObj = Instantiate(keyPrefab, parent);

            TextMeshProUGUI tmp = keyObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = letter.ToString();

            Button btn = keyObj.GetComponent<Button>();
            if (btn != null)
            {
                char captured = letter;
                btn.onClick.AddListener(() => GameManager.Instance?.OnLetterPressed(captured));
            }

            Image img = keyObj.GetComponent<Image>();
            if (img != null)
            {
                keyImages[letter] = img;
                keyStates[letter] = LetterState.Default;
                ApplyColorToKey(letter, new Color32(129, 131, 132, 255));
            }
        }

        private void CreateSpecialKey(string label, Transform parent, UnityEngine.Events.UnityAction action)
        {
            GameObject keyObj = Instantiate(keyPrefab, parent);
            TextMeshProUGUI tmp = keyObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = label;

            Button btn = keyObj.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(action);

            Image img = keyObj.GetComponent<Image>();
            if (img != null)
            {
                ApplyColorToKey(img, new Color32(129, 131, 132, 255));
            }
        }

        public void UpdateKeyStatus(char letter, LetterState newState)
        {
            letter = char.ToUpper(letter);

            if (!keyImages.ContainsKey(letter)) return;

            LetterState currentState = keyStates[letter];

            // Проверка приоритета (Зеленый > Желтый > Серый)
            if (GetStatePriority(newState) <= GetStatePriority(currentState))
                return;

            keyStates[letter] = newState;

            Color32 targetColor = newState switch
            {
                LetterState.Green => new Color32(106, 170, 100, 255),
                LetterState.Yellow => new Color32(201, 180, 88, 255),
                LetterState.Gray => new Color32(58, 58, 60, 255),
                _ => new Color32(129, 131, 132, 255)
            };

            ApplyColorToKey(letter, targetColor);
        }

        public void ResetKeyboard()
        {
            keyStates.Clear();

            foreach (var pair in keyImages)
            {
                char letter = pair.Key;
                keyStates[letter] = LetterState.Default;
                ApplyColorToKey(letter, new Color32(129, 131, 132, 255));

                if (pair.Value != null)
                {
                    Button btn = pair.Value.GetComponent<Button>();
                    if (btn != null) btn.interactable = true;
                }
            }
        }

        // Принудительно меняет цвет и у Image, и у компонента Button
        private void ApplyColorToKey(char letter, Color32 color)
        {
            if (keyImages.TryGetValue(letter, out Image img))
            {
                ApplyColorToKey(img, color);
            }
        }

        private void ApplyColorToKey(Image img, Color32 color)
        {
            if (img == null) return;

            img.color = color;

            Button btn = img.GetComponent<Button>();
            if (btn != null)
            {
                ColorBlock cb = btn.colors;
                cb.normalColor = color;
                cb.highlightedColor = color;
                cb.pressedColor = color;
                cb.selectedColor = color;
                btn.colors = cb;
            }
        }

        private int GetStatePriority(LetterState state)
        {
            return state switch
            {
                LetterState.Green => 3,
                LetterState.Yellow => 2,
                LetterState.Gray => 1,
                _ => 0
            };
        }
    }
}