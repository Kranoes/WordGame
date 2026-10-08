using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GuessWordGame
{
    public class LeaderboardItemUI : MonoBehaviour
    {
        [Header("UI Тексты")]
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Подсветка текущего игрока")]
        [SerializeField] private Image backgroundHighlight;
        [SerializeField] private Color defaultBgColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private Color playerBgColor = new Color(0.2f, 0.8f, 0.3f, 0.5f);

        public void SetData(int rank, LeaderboardEntry entry)
        {
            if (rankText != null) rankText.text = $"#{rank}";
            if (nameText != null) nameText.text = entry.playerName;
            if (scoreText != null) scoreText.text = entry.score.ToString();

            if (backgroundHighlight != null)
            {
                backgroundHighlight.color = entry.isCurrentPlayer ? playerBgColor : defaultBgColor;
            }
        }
    }
}