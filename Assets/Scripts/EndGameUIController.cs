using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GuessWordGame
{
    public class EndGameUIController : MonoBehaviour
    {
        [Header("Victory Panel")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private TextMeshProUGUI victoryWordText;
        [SerializeField] private TextMeshProUGUI victoryRewardText;
        [SerializeField] private TextMeshProUGUI victoryStreakText;
        [SerializeField] private Button victoryContinueButton;

        [Header("Defeat Panel")]
        [SerializeField] private GameObject defeatPanel;
        [SerializeField] private TextMeshProUGUI defeatWordText;
        [SerializeField] private TextMeshProUGUI defeatStreakText;
        [SerializeField] private Button saveStreakButton;
        [SerializeField] private Button extraAttemptButton;
        [SerializeField] private Button giveUpButton;

        private void Awake()
        {
            HideAllPanels();
        }

        public void ShowVictory(string word, int rewardCoins, int currentStreak)
        {
            HideAllPanels();

            if (victoryWordText != null) victoryWordText.text = word;
            if (victoryRewardText != null) victoryRewardText.text = $"+{rewardCoins}";
            if (victoryStreakText != null) victoryStreakText.text = currentStreak.ToString();

            victoryPanel.SetActive(true);
        }

        public void ShowDefeat(string correctWord, int currentStreak)
        {
            HideAllPanels();

            if (defeatWordText != null) defeatWordText.text = correctWord;
            if (defeatStreakText != null) defeatStreakText.text = $"Текущая серия: {currentStreak}";

            defeatPanel.SetActive(true);
        }

        public void HideAllPanels()
        {
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (defeatPanel != null) defeatPanel.SetActive(false);
        }
    }
}