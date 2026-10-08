using System;
using System.Globalization;

namespace GuessWordGame
{
    /// <summary>
    /// Единые "игровые" дата и время для всех игроков (МСК, UTC+3):
    /// ежедневное слово, награды и серии меняются в 00:00 по Москве.
    /// </summary>
    public static class GameClock
    {
        private static readonly TimeSpan Offset = TimeSpan.FromHours(3);

        public static DateTime Now => DateTime.UtcNow + Offset;
        public static DateTime Today => Now.Date;
        public static string TodayString => Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
