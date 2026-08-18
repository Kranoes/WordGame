using System;
using UnityEngine;

namespace WordleGame
{
    public class DailyRewardService : MonoBehaviour
    {
        public int CurrentStreak => SaveManager.CurrentData?.loginStreak ?? 0;

        public bool CanClaimToday()
        {
            if (SaveManager.CurrentData == null) return false;

            string lastClaimString = SaveManager.CurrentData.lastClaimedRewardDate;
            if (string.IsNullOrEmpty(lastClaimString))
            {
                return true;
            }

            if (DateTime.TryParse(lastClaimString, out DateTime lastClaimTime))
            {
                return DateTime.UtcNow.Date > lastClaimTime.Date;
            }

            return true;
        }

        public void CheckStreakReset()
        {
            if (SaveManager.CurrentData == null) return;

            string lastClaimString = SaveManager.CurrentData.lastClaimedRewardDate;
            if (string.IsNullOrEmpty(lastClaimString))
            {
                return;
            }

            if (DateTime.TryParse(lastClaimString, out DateTime lastClaimTime))
            {
                if ((DateTime.UtcNow.Date - lastClaimTime.Date).Days > 1)
                {
                    SaveManager.CurrentData.loginStreak = 0;
                    SaveManager.Save();
                }
            }
        }

        public void ClaimCurrentReward(int amount, bool isRuby)
        {
            if (!CanClaimToday() || SaveManager.CurrentData == null)
            {
                return;
            }

            if (isRuby)
            {
                SaveManager.CurrentData.rubies += amount;
            }
            else
            {
                SaveManager.CurrentData.coins += amount;
            }

            SaveManager.CurrentData.loginStreak = (CurrentStreak + 1) % 7;
            SaveManager.CurrentData.lastClaimedRewardDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            SaveManager.Save();
            UIManager.Instance?.UpdateCurrencyUI();
        }
    }
}