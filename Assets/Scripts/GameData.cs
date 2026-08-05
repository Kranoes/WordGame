using System;
using System.Collections.Generic;
using UnityEngine;

namespace WordleGame
{
    [Serializable]
    public class GameData
    {
        // Экономика
        public int coins;
        public int rubies; // Премиум-валюта

        // Статистика
        public int currentWinStreak;                 // Имя приведено в соответствие с StatisticsUI
        public int maxWinStreak;                     // Рекордная серия
        public int totalWins;
        public int gamesPlayed;                      // Имя приведено в соответствие с StatisticsUI
        public int[] guessDistribution = new int[6]; // Победы по 1..6 попыткам
                                                     // Настройки аудио и отклика
                                                     // Аудио (значения от 0.0f до 1.0f)
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public float uiSfxVolume = 1f;

        public bool isVibrationEnabled = true;
        public bool isLowGraphics = false;
        public List<string> redeemedPromoCodes = new List<string>();

        // Магазин и кастомизация
        public string currentActiveSkinID = "Default";
        public List<string> unlockedSkinIDs = new List<string>() { "Default" };

        // Ограничения кейсов 
        public int freeCasesOpenedToday;
        public string lastLoginDate = "";
        public string lastDailyCaseDate = "";     // Формат "YYYY-MM-DD"
        public string lastFreePlayCaseDate = "";  // Формат "YYYY-MM-DD"

        // Гарантируем, что массив инициализирован при создании объекта в памяти
        public GameData()
        {
            if (guessDistribution == null || guessDistribution.Length != 6)
            {
                guessDistribution = new int[6];
            }
        }
    }
}