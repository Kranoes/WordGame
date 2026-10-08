namespace GuessWordGame
{
    public enum LeaderboardCategory
    {
        Wins,
        Coins,
        Rubies,
        Streak
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
        public bool isCurrentPlayer;

        public LeaderboardEntry(string name, int score, bool isCurrentPlayer = false)
        {
            this.playerName = name;
            this.score = score;
            this.isCurrentPlayer = isCurrentPlayer;
        }
    }
}