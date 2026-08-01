using UnityEngine;
using System;

namespace WordleGame
{
    public static class RewardManager
    {
        //  онстанты наград за вход
        private const int LoginCoinsReward = 100;
        private const int LoginRubiesReward = 10;

        /// <summary>
        /// ѕровер€ет наступил ли новый день и выдает награду за вход (монеты + рубины)
        /// </summary>
        public static void CheckAndGrantDailyLogin()
        {
            if (SaveManager.CurrentData == null) return;

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // ≈сли дата последнего входа не совпадает с сегодн€шней Ч выдаем награду
            if (SaveManager.CurrentData.lastLoginDate != today)
            {
                SaveManager.CurrentData.coins += LoginCoinsReward;
                SaveManager.CurrentData.rubies += LoginRubiesReward;

                // ‘иксируем дату входа
                SaveManager.CurrentData.lastLoginDate = today;

                // Ќа вс€кий случай гарантируем сброс счетчика кейсов при смене дн€
                SaveManager.CurrentData.freeCasesOpenedToday = 0;

                SaveManager.Save();
                UIManager.Instance?.UpdateCurrencyUI();

                Debug.Log($"[RewardManager] Ќаграда за вход получена: +{LoginCoinsReward} монет, +{LoginRubiesReward} рубинов.");
            }
        }

        /// <summary>
        /// ¬ыдает ежедневный кейс за победу в Daily режиме (много монет)
        /// </summary>
        public static void GrantDailyWinReward(int attempt, out int droppedCoins)
        {
            droppedCoins = GetCoinsByAttempt(attempt);
            SaveManager.CurrentData.coins += droppedCoins;
            SaveManager.CurrentData.lastDailyCaseDate = System.DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// ¬ыдает обычный кейс за победу в безлимитном режиме (с лимитом 5 штук в день)
        /// </summary>
        public static bool TryGrantFreePlayReward(int attempt, out int droppedCoins)
        {
            string today = System.DateTime.UtcNow.ToString("yyyy-MM-dd");
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
                return true;
            }

            droppedCoins = 0;
            return false;
        }
        public static int GetCoinsByAttempt(int attempt)
        {
            return attempt switch
            {
                1 => UnityEngine.Random.Range(35, 51), // 1 попытка: 35Ц50 монет
                2 => UnityEngine.Random.Range(20, 31), // 2 попытка: 20Ц30 монет
                3 => UnityEngine.Random.Range(15, 21), // 3 попытка: 15Ц20 монет
                4 => UnityEngine.Random.Range(11, 16), // 4 попытка: 11Ц15 монет
                5 => UnityEngine.Random.Range(8, 12),  // 5 попытка: 8Ц11 монет
                6 => UnityEngine.Random.Range(5, 10),  // 6 попытка: 5Ц9 монет
                _ => UnityEngine.Random.Range(5, 10)
            };
        }
    }
}