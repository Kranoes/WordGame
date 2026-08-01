using UnityEngine;
using TMPro;

namespace WordleGame
{
    public class HintManager : MonoBehaviour
    {
        public static HintManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject hintsRootContainer;
        [SerializeField] private GameObject hintsContentPanel;
        [SerializeField] private TextMeshProUGUI openLetterCostText;
        [SerializeField] private TextMeshProUGUI removeLettersCostText;
        [SerializeField] private TextMeshProUGUI extraAttemptCostText;

        [Header("Настройки стоимости")]
        [SerializeField] private int openLetterCost = 25;
        [SerializeField] private int removeLettersCost = 15;
        [SerializeField] private int extraAttemptCost = 40;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            UpdateCostTexts();
            CloseHintsPanel();
        }

        private void UpdateCostTexts()
        {
            if (openLetterCostText != null)
                openLetterCostText.text = $"{openLetterCost}";

            if (removeLettersCostText != null)
                removeLettersCostText.text = $"{removeLettersCost}";

            if (extraAttemptCostText != null)
                extraAttemptCostText.text = $"{extraAttemptCost}";
        }

        public void UpdateHintButtonsVisibility(GameMode mode)
        {
            bool isFreePlay = (mode == GameMode.FreePlay);

            if (hintsRootContainer != null)
            {
                hintsRootContainer.SetActive(isFreePlay);
            }

            CloseHintsPanel();
        }

        public void ToggleHintsPanel()
        {
            if (hintsContentPanel != null)
            {
                hintsContentPanel.SetActive(!hintsContentPanel.activeSelf);
            }
        }

        public void CloseHintsPanel()
        {
            if (hintsContentPanel != null)
            {
                hintsContentPanel.SetActive(false);
            }
        }

        public void UseOpenLetterHint()
        {
            if (!ValidateHintUsage(openLetterCost)) return;

            bool success = GameManager.Instance.RevealRandomLetter();
            ProcessHintResult(success, openLetterCost, "Нельзя применить подсказку!");
        }

        public void UseRemoveLettersHint()
        {
            if (!ValidateHintUsage(removeLettersCost)) return;

            bool success = GameManager.Instance.RemoveIncorrectKeyboardLetters(3);
            ProcessHintResult(success, removeLettersCost, "Нет подходящих букв для удаления!");
        }

        public void UseExtraAttemptHint()
        {
            if (!ValidateHintUsage(extraAttemptCost)) return;

            bool success = GameManager.Instance.GrantExtraAttempt();
            ProcessHintResult(success, extraAttemptCost, "Вы еще не совершили ни одной попытки!");
        }

        private bool ValidateHintUsage(int cost)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentMode != GameMode.FreePlay)
            {
                UIManager.Instance?.ShowInfoModal("ПОДСКАЗКА", "Подсказки доступны только в свободном режиме!");
                return false;
            }

            if (!GameManager.Instance.IsGameActive)
            {
                return false;
            }

            if (SaveManager.CurrentData == null || SaveManager.CurrentData.coins < cost)
            {
                UIManager.Instance?.ShowInfoModal("НЕДОСТАТОЧНО МОНЕТ", $"Для использования требуется {cost} монет.");
                return false;
            }

            return true;
        }

        private void ProcessHintResult(bool success, int cost, string failMessage)
        {
            if (success)
            {
                SaveManager.CurrentData.coins -= cost;
                SaveManager.Save();
                UIManager.Instance?.UpdateCurrencyUI();
                CloseHintsPanel();
            }
            else
            {
                UIManager.Instance?.ShowInfoModal("ПОДСКАЗКА", failMessage);
            }
        }
    }
}