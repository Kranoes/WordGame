using System;
using UnityEngine;

namespace WordleGame
{
    public struct DailyReward
    {
        public int Day;
        public int Coins;
        public int Rubies;

        public DailyReward(int day, int coins, int rubies)
        {
            Day = day;
            Coins = coins;
            Rubies = rubies;
        }
    }

    public static class DailyRewardManager
    {
        // Таблица 7-дневной сетки наград
        private static readonly DailyReward[] Rewards = new DailyReward[]
 {
    new DailyReward(1, 100, 0),  // День 1: 100 монет
    new DailyReward(2, 150, 0),  // День 2: 150 монет
    new DailyReward(3, 0, 3),    // День 3: 3 рубина (Промежуточный чекпоинт)
    new DailyReward(4, 200, 0),  // День 4: 200 монет
    new DailyReward(5, 250, 0),  // День 5: 250 монет
    new DailyReward(6, 300, 0),  // День 6: 300 монет
    new DailyReward(7, 0, 10)    // День 7: 10 рубинов (Главная награда недели)
 };

        public static string TodayDate => DateTime.UtcNow.ToString("yyyy-MM-dd");

        /// <summary>
        /// Доступна ли сегодня награда
        /// </summary>
        public static bool CanClaimReward()
        {
            var data = SaveManager.CurrentData;
            if (data == null) return false;

            return data.lastClaimedRewardDate != TodayDate;
        }

        /// <summary>
        /// Возвращает текущий день серии (1..7), который игрок заберет сегодня
        /// </summary>
        public static int GetCurrentStreakDay()
        {
            var data = SaveManager.CurrentData;
            if (data == null) return 1;

            if (string.IsNullOrEmpty(data.lastClaimedRewardDate))
                return 1;

            if (DateTime.TryParse(data.lastClaimedRewardDate, out DateTime lastDate))
            {
                int daysDifference = (DateTime.UtcNow.Date - lastDate.Date).Days;

                if (daysDifference == 1)
                {
                    // Входил вчера — продолжается серия (после 7 сбрасывается на 1)
                    int nextDay = data.loginStreak + 1;
                    return nextDay > 7 ? 1 : nextDay;
                }
                else if (daysDifference > 1)
                {
                    // Пропустил хотя бы день — серия сбрасывается
                    return 1;
                }
            }

            return data.loginStreak == 0 ? 1 : data.loginStreak;
        }

        public static DailyReward GetRewardForDay(int day)
        {
            int index = Mathf.Clamp(day - 1, 0, Rewards.Length - 1);
            return Rewards[index];
        }

        /// <summary>
        /// Забрать награду за текущий день
        /// </summary>
        public static bool ClaimReward(out DailyReward grantedReward)
        {
            grantedReward = default;
            if (!CanClaimReward()) return false;

            var data = SaveManager.CurrentData;
            int currentDay = GetCurrentStreakDay();
            grantedReward = GetRewardForDay(currentDay);

            // Начисляем валюту
            data.coins += grantedReward.Coins;
            data.rubies += grantedReward.Rubies;

            // Обновляем серию и дату
            data.loginStreak = currentDay;
            data.lastClaimedRewardDate = TodayDate;

            // Сохраняем
            SaveManager.Save();
            UIManager.Instance?.UpdateCurrencyUI();

            return true;
        }
    }
}