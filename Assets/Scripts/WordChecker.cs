using System.Collections.Generic;

namespace WordleGame
{
    public static class WordChecker
    {
        public static LetterState[] EvaluateWord(string guessWord, string targetWord)
        {
            int length = guessWord.Length;
            LetterState[] results = new LetterState[length];
            char[] targetChars = targetWord.ToCharArray();
            bool[] targetUsed = new bool[length];

            // 1. Точные совпадения (Зеленые)
            for (int i = 0; i < length; i++)
            {
                if (guessWord[i] == targetChars[i])
                {
                    results[i] = LetterState.Green;
                    targetUsed[i] = true;
                }
            }

            // 2. Частичные совпадения (Желтые)
            for (int i = 0; i < length; i++)
            {
                if (results[i] == LetterState.Green) continue;

                for (int j = 0; j < length; j++)
                {
                    if (!targetUsed[j] && guessWord[i] == targetChars[j])
                    {
                        results[i] = LetterState.Yellow;
                        targetUsed[j] = true;
                        break;
                    }
                }

                if (results[i] != LetterState.Yellow)
                {
                    results[i] = LetterState.Gray;
                }
            }

            return results;
        }
    }
}