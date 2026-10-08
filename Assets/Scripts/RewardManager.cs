using System;
using UnityEngine;

namespace GuessWordGame
{
    public static class RewardManager
    {
        public static (int amount, bool isRuby) GetDailyRewardConfig(int dayIndex)
        {
            return dayIndex switch
            {
                0 => (100, false),
                1 => (150, false),
                2 => (3, true),
                3 => (200, false),
                4 => (250, false),
                5 => (300, false),
                6 => (10, true),
                _ => (100, false)
            };
        }

        public static void CheckAndUpdateStreak()
        {
            if (SaveManager.CurrentData == null) return;

            string lastClaimStr = SaveManager.CurrentData.lastClaimedRewardDate;
            if (string.IsNullOrEmpty(lastClaimStr)) return;

            if (DateTime.TryParse(lastClaimStr, out DateTime lastClaimDate))
            {
                int daysPassed = (GameClock.Today - lastClaimDate.Date).Days;
                if (daysPassed > 1)
                {
                    SaveManager.CurrentData.loginStreak = 0;
                    SaveManager.Save();
                }
            }
        }

        public static bool CanClaimDailyReward()
        {
            if (SaveManager.CurrentData == null) return false;

            string today = GameClock.TodayString;
            return SaveManager.CurrentData.lastClaimedRewardDate != today;
        }

        public static bool ClaimDailyReward()
        {
            if (SaveManager.CurrentData == null || !CanClaimDailyReward()) return false;

            CheckAndUpdateStreak();

            int currentStreak = SaveManager.CurrentData.loginStreak;
            var (amount, isRuby) = GetDailyRewardConfig(currentStreak);

            if (isRuby)
                SaveManager.CurrentData.rubies += amount;
            else
                SaveManager.CurrentData.coins += amount;

            SaveManager.CurrentData.lastClaimedRewardDate = GameClock.TodayString;
            SaveManager.CurrentData.loginStreak = (currentStreak + 1) % 7;

            SaveManager.Save();
            UIManager.Instance?.UpdateCurrencyUI();

            return true;
        }

        public static bool CanGetFreePlayReward()
        {
            if (SaveManager.CurrentData == null) return false;

            string today = GameClock.TodayString;
            if (SaveManager.CurrentData.lastFreePlayCaseDate != today)
            {
                SaveManager.CurrentData.lastFreePlayCaseDate = today;
                SaveManager.CurrentData.freeCasesOpenedToday = 0;
                SaveManager.Save();
            }

            return SaveManager.CurrentData.freeCasesOpenedToday < 5;
        }

        public static bool GrantWinReward(GameMode mode, int attempt, out int droppedCoins)
        {
            droppedCoins = 0;
            if (SaveManager.CurrentData == null) return false;

            string today = GameClock.TodayString;

            if (mode == GameMode.Daily)
            {
                if (SaveManager.CurrentData.lastDailyCaseDate == today)
                    return false;

                droppedCoins = GetCoinsByAttempt(attempt);
                SaveManager.CurrentData.coins += droppedCoins;
                SaveManager.CurrentData.lastDailyCaseDate = today;

                SaveManager.Save();
                UIManager.Instance?.UpdateCurrencyUI();
                return true;
            }
            else if (mode == GameMode.FreePlay)
            {
                if (CanGetFreePlayReward())
                {
                    droppedCoins = GetCoinsByAttempt(attempt);
                    SaveManager.CurrentData.coins += droppedCoins;
                    SaveManager.CurrentData.freeCasesOpenedToday++;

                    SaveManager.Save();
                    UIManager.Instance?.UpdateCurrencyUI();
                    return true;
                }
            }

            return false;
        }

        public static int GetCoinsByAttempt(int attempt)
        {
            return attempt switch
            {
                1 => UnityEngine.Random.Range(35, 51),
                2 => UnityEngine.Random.Range(20, 31),
                3 => UnityEngine.Random.Range(15, 21),
                4 => UnityEngine.Random.Range(11, 16),
                5 => UnityEngine.Random.Range(8, 12),
                6 => UnityEngine.Random.Range(5, 10),
                _ => UnityEngine.Random.Range(5, 10)
            };
        }
    }
}