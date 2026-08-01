using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WordleGame
{
    public class StatisticsUI : MonoBehaviour
    {
        [Header("Основные счетчики")]
        [SerializeField] private TextMeshProUGUI gamesPlayedText;
        [SerializeField] private TextMeshProUGUI winRateText;
        [SerializeField] private TextMeshProUGUI currentStreakText;
        [SerializeField] private TextMeshProUGUI maxStreakText;

        [Header("Гистограмма попыток (1-6)")]
        [SerializeField] private TextMeshProUGUI[] guessCountTexts; // Массив из 6 текстов
        [SerializeField] private Image[] guessFillBars;            // Массив из 6 полосок (Image type = Filled)

        private void OnEnable()
        {
            RefreshUI();
        }

        public void RefreshUI()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            // 1. Расчет основных метрик
            int played = data.gamesPlayed;
            int wins = data.totalWins;
            int winRate = played > 0 ? Mathf.RoundToInt((float)wins / played * 100f) : 0;

            if (gamesPlayedText != null) gamesPlayedText.text = played.ToString();
            if (winRateText != null) winRateText.text = $"{winRate}%";
            if (currentStreakText != null) currentStreakText.text = data.currentWinStreak.ToString();
            if (maxStreakText != null) maxStreakText.text = data.maxWinStreak.ToString();

            // 2. Отрисовка гистограммы распределения попыток
            UpdateGuessDistribution(data);
        }

        private void UpdateGuessDistribution(GameData data)
        {
            if (data.guessDistribution == null || data.guessDistribution.Length < 6) return;

            // Находим максимальное значение для пропорционального заполнения полосок
            int maxCount = 1;
            for (int i = 0; i < 6; i++)
            {
                if (data.guessDistribution[i] > maxCount)
                {
                    maxCount = data.guessDistribution[i];
                }
            }

            for (int i = 0; i < 6; i++)
            {
                int count = data.guessDistribution[i];

                // Обновляем число побед на данной попытке
                if (i < guessCountTexts.Length && guessCountTexts[i] != null)
                {
                    guessCountTexts[i].text = count.ToString();
                }

                // Обновляем длину полоски (fillAmount)
                if (i < guessFillBars.Length && guessFillBars[i] != null)
                {
                    // 0.08f — минимальная плашка, чтобы 0 побед выглядел аккуратной тонкой полоской
                    float fillRatio = count == 0 ? 0.08f : (float)count / maxCount;
                    guessFillBars[i].fillAmount = fillRatio;
                }
            }
        }
    }
}