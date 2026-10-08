using System;
using UnityEngine;

namespace GuessWordGame
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
        private static readonly DailyReward[] Rewards = new DailyReward[]
        {
            new DailyReward(1, 100, 0),
            new DailyReward(2, 150, 0),
            new DailyReward(3, 0, 3),
            new DailyReward(4, 200, 0),
            new DailyReward(5, 250, 0),
            new DailyReward(6, 300, 0),
            new DailyReward(7, 0, 10)
        };

        public static string TodayDate => GameClock.TodayString;

        public static bool CanClaimReward()
        {
            var data = SaveManager.CurrentData;
            if (data == null) return false;

            return data.lastClaimedRewardDate != TodayDate;
        }

        public static int GetCurrentStreakDay()
        {
            var data = SaveManager.CurrentData;
            if (data == null) return 1;

            if (string.IsNullOrEmpty(data.lastClaimedRewardDate))
                return 1;

            if (DateTime.TryParse(data.lastClaimedRewardDate, out DateTime lastDate))
            {
                int daysDifference = (GameClock.Today - lastDate.Date).Days;

                if (daysDifference == 1)
                {
                    int nextDay = data.loginStreak + 1;
                    return nextDay > 7 ? 1 : nextDay;
                }
                else if (daysDifference > 1)
                {
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

        public static bool ClaimReward(out DailyReward grantedReward)
        {
            grantedReward = default;
            if (!CanClaimReward()) return false;

            var data = SaveManager.CurrentData;
            int currentDay = GetCurrentStreakDay();
            grantedReward = GetRewardForDay(currentDay);

            data.coins += grantedReward.Coins;
            data.rubies += grantedReward.Rubies;

            data.loginStreak = currentDay;
            data.lastClaimedRewardDate = TodayDate;

            SaveManager.Save();
            UIManager.Instance?.UpdateCurrencyUI();

            return true;
        }
    }
}