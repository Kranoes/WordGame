using System;
using System.Collections.Generic;

namespace GuessWordGame
{
    [Serializable]
    public class GameData
    {
        public int coins;
        public int rubies;

        public int currentWinStreak;
        public int maxWinStreak;
        public int totalWins;
        public int gamesPlayed;
        public int[] guessDistribution = new int[6];
        public int extraAttemptHints;
        public int revealLetterHints;
        public int removeLetterHints;

        public int loginStreak = 0;
        public string lastClaimedRewardDate = "";

        public float musicVolume = 0.5f;
        public float sfxVolume = 0.5f;
        public float uiSfxVolume = 0.5f;

        public bool isVibrationEnabled = true;
        public bool isLowGraphics = false;
        public List<string> redeemedPromoCodes = new List<string>();

        public string currentActiveSkinID = "Default";
        public List<string> unlockedSkinIDs = new List<string>() { "Default" };

        public int freeCasesOpenedToday;
        public string lastLoginDate = "";
        public string lastDailyCaseDate = "";
        public string lastFreePlayCaseDate = "";

        public GameData()
        {
            if (guessDistribution == null || guessDistribution.Length != 6)
            {
                guessDistribution = new int[6];
            }
        }
    }
}