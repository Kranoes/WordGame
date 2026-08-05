using UnityEngine;
using System;

namespace WordleGame
{
    public static class RewardManager
    {
        // Константы наград за вход
        private const int LoginCoinsReward = 100;
        private const int LoginRubiesReward = 10;

        /// <summary>
        /// Проверяет наступил ли новый день и выдает награду за вход (монеты + рубины)
        /// </summary>
        public static void CheckAndGrantDailyLogin()
        {
            if (SaveManager.CurrentData == null) return;

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // Если дата последнего входа не совпадает с сегодняшней — выдаем награду
            if (SaveManager.CurrentData.lastLoginDate != today)
            {
                SaveManager.CurrentData.coins += LoginCoinsReward;
                SaveManager.CurrentData.rubies += LoginRubiesReward;

                // Фиксируем дату входа
                SaveManager.CurrentData.lastLoginDate = today;

                // На всякий случай гарантируем сброс счетчика кейсов при смене дня
                SaveManager.CurrentData.freeCasesOpenedToday = 0;

                SaveManager.Save();
                UIManager.Instance?.UpdateCurrencyUI();

                Debug.Log($"[RewardManager] Награда за вход получена: +{LoginCoinsReward} монет, +{LoginRubiesReward} рубинов.");
            }
        }

        /// <summary>
        /// Выдает ежедневный кейс за победу в Daily режиме (много монет)
        /// </summary>
        public static void GrantDailyWinReward(int attempt, out int droppedCoins)
        {
            droppedCoins = GetCoinsByAttempt(attempt);

            if (SaveManager.CurrentData == null) return;

            SaveManager.CurrentData.coins += droppedCoins;
            SaveManager.CurrentData.lastDailyCaseDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // Обязательно сохраняем и обновляем UI
            SaveManager.Save();
            UIManager.Instance?.UpdateCurrencyUI();
        }

        /// <summary>
        /// Выдает обычный кейс за победу в безлимитном режиме (с лимитом 5 штук в день)
        /// </summary>
        public static bool TryGrantFreePlayReward(int attempt, out int droppedCoins)
        {
            droppedCoins = 0;
            if (SaveManager.CurrentData == null) return false;

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (SaveManager.CurrentData.lastFreePlayCaseDate != today)
            {
                SaveManager.CurrentData.lastFreePlayCaseDate = today;
                SaveManager.CurrentData.freeCasesOpenedToday = 0;
            }

            if (SaveManager.CurrentData.freeCasesOpenedToday < 5)
            {
                droppedCoins = GetCoinsByAttempt(attempt);
                SaveManager.CurrentData.coins += droppedCoins;
                SaveManager.CurrentData.freeCasesOpenedToday++;

                // Обязательно сохраняем и обновляем UI
                SaveManager.Save();
                UIManager.Instance?.UpdateCurrencyUI();
                return true;
            }

            return false;
        }

        public static int GetCoinsByAttempt(int attempt)
        {
            return attempt switch
            {
                1 => UnityEngine.Random.Range(35, 51), // 1 попытка: 35–50 монет
                2 => UnityEngine.Random.Range(20, 31), // 2 попытка: 20–30 монет
                3 => UnityEngine.Random.Range(15, 21), // 3 попытка: 15–20 монет
                4 => UnityEngine.Random.Range(11, 16), // 4 попытка: 11–15 монет
                5 => UnityEngine.Random.Range(8, 12),  // 5 попытка: 8–11 монет
                6 => UnityEngine.Random.Range(5, 10),  // 6 попытка: 5–9 монет
                _ => UnityEngine.Random.Range(5, 10)
            };
        }
    }
}