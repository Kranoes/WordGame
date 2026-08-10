using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WordleGame
{
    public class DailyRewardCardUI : MonoBehaviour
    {
        [Header("UI Элементы")]
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI rewardAmountText;
        [SerializeField] private GameObject claimedOverlay; // Галочка или слой "Забрано"

        [Header("Спрайты валют")]
        [SerializeField] private Sprite coinSprite;
        [SerializeField] private Sprite rubySprite;

        [Header("Палитра Wordle")]
        [SerializeField] private Color greenActive = new Color32(106, 170, 100, 255); // #6AAA64
        [SerializeField] private Color yellowSpecial = new Color32(201, 180, 88, 255); // #C9B458
        [SerializeField] private Color grayLocked = new Color32(58, 58, 60, 255);    // #3A3A3C
        [SerializeField] private Color darkClaimed = new Color32(30, 30, 32, 255);    // #1E1E20

        public void Setup(int dayNumber, DailyReward reward, int currentStreak, bool isClaimedToday)
        {
            if (dayText != null)
                dayText.text = $"ДЕНЬ {dayNumber}";

            // Определение типа валюты и иконки
            bool isRubyDay = reward.Rubies > 0;
            if (rewardIcon != null)
                rewardIcon.sprite = isRubyDay ? rubySprite : coinSprite;

            if (rewardAmountText != null)
                rewardAmountText.text = isRubyDay ? $"+{reward.Rubies}" : $"+{reward.Coins}";

            // --- Состояния карточки ---

            // 1. Прошлые дни или уже забранный сегодняшний день
            if (dayNumber < currentStreak || (dayNumber == currentStreak && isClaimedToday))
            {
                if (cardBackground != null) cardBackground.color = darkClaimed;
                if (claimedOverlay != null) claimedOverlay.SetActive(true);
                transform.localScale = Vector3.one;
            }
            // 2. Текущий доступный день (можно забрать прямо сейчас)
            else if (dayNumber == currentStreak && !isClaimedToday)
            {
                if (cardBackground != null) cardBackground.color = isRubyDay ? yellowSpecial : greenActive;
                if (claimedOverlay != null) claimedOverlay.SetActive(false);
                transform.localScale = new Vector3(1.08f, 1.08f, 1f); // Легкий акцент масштабом
            }
            // 3. Будущие заблокированные дни
            else
            {
                if (cardBackground != null) cardBackground.color = grayLocked;
                if (claimedOverlay != null) claimedOverlay.SetActive(false);
                transform.localScale = Vector3.one;
            }
        }
    }
}