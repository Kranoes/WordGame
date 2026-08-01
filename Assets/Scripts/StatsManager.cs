using UnityEngine;

namespace WordleGame
{
    public class StatsManager : MonoBehaviour
    {
        public static StatsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// Обрабатывает результат завершенного раунда
        /// </summary>
        public void ProcessGameResult(bool isWin, int usedAttempts)
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            data.gamesPlayed++;

            if (isWin)
            {
                data.totalWins++;
                data.currentWinStreak++;

                if (data.currentWinStreak > data.maxWinStreak)
                {
                    data.maxWinStreak = data.currentWinStreak;
                }

                // Запись попытки в распределение (0 = 1-я попытка)
                if (data.guessDistribution == null || data.guessDistribution.Length != 6)
                {
                    data.guessDistribution = new int[6];
                }

                int attemptIndex = Mathf.Clamp(usedAttempts - 1, 0, 5);
                data.guessDistribution[attemptIndex]++;
            }
            else
            {
                data.currentWinStreak = 0;
            }

            SaveManager.Save();
        }

        /// <summary>
        /// Возвращает процент побед
        /// </summary>
        public float GetWinRate()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null || data.gamesPlayed == 0) return 0f;

            return ((float)data.totalWins / data.gamesPlayed) * 100f;
        }
    }
}