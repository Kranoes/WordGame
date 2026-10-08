using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuessWordGame
{
    public class StatisticsUI : MonoBehaviour
    {
        [Header("Основные счетчики")]
        [SerializeField] private TextMeshProUGUI gamesPlayedText;
        [SerializeField] private TextMeshProUGUI winRateText;
        [SerializeField] private Image winRateFillBar; // Шкала процента побед
        [SerializeField] private TextMeshProUGUI currentStreakText;
        [SerializeField] private TextMeshProUGUI maxStreakText;

        [Header("Гистограмма попыток (1-6)")]
        [SerializeField] private TextMeshProUGUI[] guessCountTexts; // 6 текстов (% побед)
        [SerializeField] private Image[] guessFillBars;             // 6 полосок заполнения

        private void OnEnable()
        {
            RefreshUI();
        }

        public void RefreshUI()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            // 1. Единый источник правды: считаем РЕАЛЬНОЕ число побед по строкам гистограммы
            int actualWins = 0;
            if (data.guessDistribution != null)
            {
                for (int i = 0; i < Mathf.Min(6, data.guessDistribution.Length); i++)
                {
                    actualWins += Mathf.Max(0, data.guessDistribution[i]);
                }
            }

            // Исправляем рассинхронизировавшийся totalWins в объекте сохранения
            data.totalWins = actualWins;

            // 2. Валидация сыгранных игр
            int played = Mathf.Max(0, data.gamesPlayed);
            if (played < actualWins) played = actualWins; // Сыграно не может быть меньше побед

            // 3. Расчет Винрейта: Процент побед от ВСЕХ сыгранных игр (например, 4 / 5 = 80%)
            float winRateRatio = played > 0 ? (float)actualWins / played : 0f;
            int winRatePercentage = Mathf.RoundToInt(winRateRatio * 100f);

            // 4. Ограничение серий
            int currentStreak = Mathf.Clamp(data.currentWinStreak, 0, played);
            int maxStreak = Mathf.Clamp(data.maxWinStreak, 0, played);

            // 5. Вывод основных значений
            if (gamesPlayedText != null) gamesPlayedText.text = played.ToString();
            if (winRateText != null) winRateText.text = $"{winRatePercentage}%";
            if (currentStreakText != null) currentStreakText.text = currentStreak.ToString();
            if (maxStreakText != null) maxStreakText.text = maxStreak.ToString();

            if (winRateFillBar != null)
            {
                winRateFillBar.fillAmount = winRateRatio;
            }

            // 6. Отрисовка гистограммы (строго от количества ПОБЕД actualWins)
            UpdateGuessDistribution(data, actualWins);
        }

        private void UpdateGuessDistribution(GameData data, int totalWins)
        {
            if (data.guessDistribution == null || data.guessDistribution.Length < 6) return;

            for (int i = 0; i < 6; i++)
            {
                int count = Mathf.Max(0, data.guessDistribution[i]);

                // Доля текущей попытки строго от ОБЩЕГО ЧИСЛА ПОБЕД (а не сыгранных игр!)
                // Если побед 4: 2/4 = 50%, 1/4 = 25%, 1/4 = 25%. В сумме всегда 100%!
                float shareRatio = totalWins > 0 ? (float)count / totalWins : 0f;
                int sharePercentage = Mathf.RoundToInt(shareRatio * 100f);

                // Текст процента
                if (guessCountTexts != null && i < guessCountTexts.Length && guessCountTexts[i] != null)
                {
                    guessCountTexts[i].text = $"{sharePercentage}%";
                }

                // Заполнение полоски (min 0.08f для красивой плашки при 0%)
                if (guessFillBars != null && i < guessFillBars.Length && guessFillBars[i] != null)
                {
                    float fillRatio = count == 0 ? 0.08f : Mathf.Clamp(shareRatio, 0.08f, 1f);
                    guessFillBars[i].fillAmount = fillRatio;
                }
            }
        }
    }
}