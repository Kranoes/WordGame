using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GuessWordGame
{
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public List<LeaderboardEntry> GetLeaderboard(LeaderboardCategory category)
        {
            GameData data = SaveManager.CurrentData;
            int playerScore = 0;

            if (data != null)
            {
                playerScore = category switch
                {
                    LeaderboardCategory.Wins => data.totalWins,
                    LeaderboardCategory.Coins => data.coins,
                    LeaderboardCategory.Rubies => data.rubies,
                    LeaderboardCategory.Streak => data.maxWinStreak,
                    _ => 0
                };
            }

            List<LeaderboardEntry> entries = GetBotEntries(category);
            entries.Add(new LeaderboardEntry("Вы", playerScore, true));

            return entries.OrderByDescending(e => e.score).ToList();
        }

        private List<LeaderboardEntry> GetBotEntries(LeaderboardCategory category)
        {
            int multiplier = category switch
            {
                LeaderboardCategory.Wins => 10,
                LeaderboardCategory.Coins => 150,
                LeaderboardCategory.Rubies => 25,
                LeaderboardCategory.Streak => 3,
                _ => 10
            };

            return new List<LeaderboardEntry>
            {
                new LeaderboardEntry("Alex_WordMaster", 45 * multiplier),
                new LeaderboardEntry("Elena_Pro", 38 * multiplier),
                new LeaderboardEntry("WordKing99", 29 * multiplier),
                new LeaderboardEntry("Slayer_3000", 21 * multiplier),
                new LeaderboardEntry("Vadim_Dev", 15 * multiplier),
                new LeaderboardEntry("Ghost_Rider", 8 * multiplier),
                new LeaderboardEntry("Newbie_007", 3 * multiplier)
            };
        }
    }
}