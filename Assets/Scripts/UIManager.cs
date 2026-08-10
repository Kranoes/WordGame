using System.Collections;
using TMPro;
using UnityEngine;

namespace WordleGame
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Result Modal Window")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Transform resultCardTransform;
        [SerializeField] private TextMeshProUGUI resultTitleText;
        [SerializeField] private TextMeshProUGUI resultSecretWordText;
        [SerializeField] private TextMeshProUGUI resultRewardText;
        [SerializeField] private TextMeshProUGUI resultStreakText;

        [Header("Panels & Controls")]
        [SerializeField] private GameObject statsPanel;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject homeButton;

        [Header("Main Menu UI")]
        [SerializeField] private GameObject chooseText;

        [Header("Currency UI")]
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI rubiesText;

        [Header("Status Text & Animations")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI streakText;

        [Header("Leaderboard Panel")]
        [SerializeField] private GameObject leaderboardPanel;

        [Header("Settings Panel")]
        [SerializeField] private GameObject settingsPanel;

        [SerializeField] private DailyRewardPanelUI dailyRewardPanel;

        private readonly string[] winMessages = { "Хорошая работа!", "Так держать!", "Блестяще!", "Гениально!", "Вы мастер слов!" };
        private readonly string[] loseMessages = { "Не повезло...", "Попробуй ещё раз!", "Упс, почти получилось!" };

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        private void Start()
        {
            UpdateCurrencyUI();
            UpdateStreakUI();
            if (statusText != null) statusText.text = "";
            HideResultModal();
            ShowMainMenu(true);

            // Гарантированно скрываем модальные панели на старте
            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            if (statsPanel != null)
                statsPanel.SetActive(false);
            if (DailyRewardManager.CanClaimReward())
            {
                if (dailyRewardPanel != null)
                {
                    dailyRewardPanel.gameObject.SetActive(true);
                }
            }
        }
        public void ShowSettingsModal(bool show)
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(show);
            }
            else
            {
                Debug.LogError("[UIManager] settingsPanel РАВЕН NULL! Назначь его в Инспекторе на объекте UIManager!");
            }
        }
        public void OpenSettingsModal()
        {
            ShowSettingsModal(true);
        }

        public void CloseSettingsModal()
        {
            ShowSettingsModal(false);
        }
        public void ShowLeaderboard(bool show)
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(show);

                // При открытии окна обновляем в нем данные
                if (show)
                {
                    LeaderboardUI ui = leaderboardPanel.GetComponent<LeaderboardUI>();
                    if (ui != null) ui.RefreshUI();
                }
            }
        }

        // Метод для привязки на кнопку закрытия лидерборда в инспекторе
        public void CloseLeaderboard()
        {
            ShowLeaderboard(false);
        }

        // ПРЯМЫЕ МЕТОДЫ ДЛЯ КНОПОК СТАТИСТИКИ
        public void OpenStatsModal()
        {
            ShowStatsModal(true);
        }

        public void CloseStatsModal()
        {
            ShowStatsModal(false);
        }

        public void ShowStatsModal(bool show)
        {
            if (statsPanel != null)
            {
                statsPanel.SetActive(show);
            }
            else
            {
                Debug.LogError("[UIManager] statsPanel РАВЕН NULL! Назначь его в Инспекторе на объекте UIManager!");
            }
        }

        public void ShowWinModal(string rewardMessage, int currentStreak)
        {
            if (resultPanel == null) return;

            resultTitleText.text = "ПОБЕДА!";
            resultTitleText.color = new Color(0.42f, 0.67f, 0.39f);

            resultSecretWordText.gameObject.SetActive(false);
            resultRewardText.text = rewardMessage;

            if (currentStreak > 0)
                resultStreakText.text = $"Серия побед: {currentStreak}";
            else
                resultStreakText.text = "";

            StartCoroutine(AnimateModalShow());
        }

        public void ShowLoseModal(string secretWord)
        {
            if (resultPanel == null) return;

            resultTitleText.text = "ПОРАЖЕНИЕ";
            resultTitleText.color = new Color(0.86f, 0.31f, 0.31f);

            resultSecretWordText.gameObject.SetActive(true);
            resultSecretWordText.text = $"Загаданное слово:\n<b><color=#FFFFFF>{secretWord.ToUpper()}</color></b>";

            resultRewardText.text = "Попробуйте ещё раз!";
            resultStreakText.text = "Серия сброшена";

            StartCoroutine(AnimateModalShow());
        }

        public void HideResultModal()
        {
            if (resultPanel != null)
                resultPanel.SetActive(false);
        }

        private IEnumerator AnimateModalShow()
        {
            resultPanel.SetActive(true);

            if (resultCardTransform != null)
            {
                resultCardTransform.localScale = Vector3.zero;
                Vector3 overshootScale = new Vector3(1.08f, 1.08f, 1.08f);
                Vector3 targetScale = Vector3.one;

                float duration = 0.25f;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    resultCardTransform.localScale = Vector3.Lerp(Vector3.zero, overshootScale, elapsed / duration);
                    yield return null;
                }

                elapsed = 0f;
                duration = 0.1f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    resultCardTransform.localScale = Vector3.Lerp(overshootScale, targetScale, elapsed / duration);
                    yield return null;
                }

                resultCardTransform.localScale = targetScale;
            }
        }

        public void ShowInfoModal(string title, string message)
        {
            if (resultPanel == null) return;

            if (resultTitleText != null)
            {
                resultTitleText.text = title.ToUpper();
                resultTitleText.color = Color.white;
            }

            if (resultSecretWordText != null)
                resultSecretWordText.gameObject.SetActive(false);

            if (resultRewardText != null)
                resultRewardText.text = message;

            if (resultStreakText != null)
                resultStreakText.text = "";

            StartCoroutine(AnimateModalShow());
        }

        public void UpdateCurrencyUI()
        {
            if (SaveManager.CurrentData != null)
            {
                UpdateCurrencyUI(SaveManager.CurrentData.coins, SaveManager.CurrentData.rubies);
            }
        }

        public void UpdateCurrencyUI(int coins, int rubies)
        {
            if (coinsText != null)
                coinsText.text = coins.ToString();

            if (rubiesText != null)
                rubiesText.text = rubies.ToString();
        }

        public void UpdateStreakUI()
        {
            int currentStreak = SaveManager.CurrentData != null ? SaveManager.CurrentData.currentWinStreak : 0;

            if (streakText != null)
            {
                streakText.text = $"Серия побед: {currentStreak}";
            }
        }

        public void ShowMainMenu(bool show)
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(show);

            if (chooseText != null)
                chooseText.SetActive(show);

            if (show)
            {
                UpdateStreakUI();
            }
        }

        public void ShowWinStatus(string customPrefix = "")
        {
            if (statusText == null) return;
            string randomPhrase = winMessages[Random.Range(0, winMessages.Length)];
            string finalMessage = string.IsNullOrEmpty(customPrefix) ? randomPhrase : $"{customPrefix}\n{randomPhrase}";
            StartCoroutine(AnimateStatusText(finalMessage, Color.green));
        }

        public void ShowLoseStatus(string secretWord)
        {
            if (statusText == null) return;
            string randomPhrase = loseMessages[Random.Range(0, loseMessages.Length)];
            string finalMessage = $"Слово: {secretWord}\n{randomPhrase}";
            StartCoroutine(AnimateStatusText(finalMessage, Color.red));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (statsPanel != null && statsPanel.activeSelf)
                {
                    CloseStatsModal();
                }
                else if (leaderboardPanel != null && leaderboardPanel.activeSelf)
                {
                    CloseLeaderboard();
                }
                else if (resultPanel != null && resultPanel.activeSelf)
                {
                    HideResultModal();
                }
                else if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    CloseSettingsModal();
                }
            }
        }

        public void ClearStatusText()
        {
            if (statusText != null) statusText.text = "";
        }

        private IEnumerator AnimateStatusText(string text, Color targetColor)
        {
            statusText.text = text;
            statusText.color = targetColor;

            Vector3 startScale = Vector3.zero;
            Vector3 overshootScale = new Vector3(1.2f, 1.2f, 1.2f);
            Vector3 finalScale = Vector3.one;

            float duration = 0.2f;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                statusText.transform.localScale = Vector3.Lerp(startScale, overshootScale, time / duration);
                yield return null;
            }

            time = 0f;
            duration = 0.1f;
            while (time < duration)
            {
                time += Time.deltaTime;
                statusText.transform.localScale = Vector3.Lerp(overshootScale, finalScale, time / duration);
                yield return null;
            }
        }
    }
}