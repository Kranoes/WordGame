using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuessWordGame
{
    public class DailyRewardCardUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image cardBackground;
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TextMeshProUGUI rewardAmountText;
        [SerializeField] private Button claimButton;
        [SerializeField] private TextMeshProUGUI claimButtonText;
        [SerializeField] private GameObject claimedOverlay;
        [SerializeField] private GameObject lockIcon;

        [Header("Sprites")]
        [SerializeField] private Sprite coinSprite;
        [SerializeField] private Sprite rubySprite;

        [Header("Card Background Colors")]
        [SerializeField] private Color activeCardColor = new Color32(50, 45, 30, 255);
        [SerializeField] private Color lockedCardColor = new Color32(35, 35, 35, 255);
        [SerializeField] private Color claimedCardColor = new Color32(25, 25, 25, 200);

        [Header("Button Colors")]
        [SerializeField] private Color activeBtnColor = new Color32(46, 204, 113, 255);
        [SerializeField] private Color lockedBtnColor = new Color32(70, 70, 70, 255);

        public void Setup(int dayNumber, int amount, bool isRuby, RewardState state, Action onClaim)
        {
            if (dayText != null)
                dayText.text = $"День {dayNumber}";

            if (rewardAmountText != null)
                rewardAmountText.text = $"+{amount}";

            if (rewardIcon != null)
                rewardIcon.sprite = isRuby ? rubySprite : coinSprite;

            if (claimButton != null)
                claimButton.onClick.RemoveAllListeners();

            Image btnImg = claimButton != null ? claimButton.GetComponent<Image>() : null;

            switch (state)
            {
                case RewardState.ReadyToClaim:
                    if (cardBackground != null) cardBackground.color = activeCardColor;
                    if (lockIcon != null) lockIcon.SetActive(false);
                    if (claimedOverlay != null) claimedOverlay.SetActive(false);

                    if (claimButton != null)
                    {
                        claimButton.gameObject.SetActive(true);
                        claimButton.interactable = true;
                        if (btnImg != null) btnImg.color = activeBtnColor;
                        if (claimButtonText != null)
                        {
                            claimButtonText.gameObject.SetActive(true);
                            claimButtonText.text = "Получить";
                        }

                        claimButton.onClick.AddListener(() => onClaim?.Invoke());
                    }
                    break;

                case RewardState.Claimed:
                    if (cardBackground != null) cardBackground.color = claimedCardColor;
                    if (lockIcon != null) lockIcon.SetActive(false);
                    if (claimedOverlay != null) claimedOverlay.SetActive(true);

                    if (claimButton != null) claimButton.gameObject.SetActive(false);
                    break;

                case RewardState.Locked:
                    if (cardBackground != null) cardBackground.color = lockedCardColor;
                    if (lockIcon != null) lockIcon.SetActive(true);
                    if (claimedOverlay != null) claimedOverlay.SetActive(false);

                    if (claimButton != null)
                    {
                        claimButton.gameObject.SetActive(true);
                        claimButton.interactable = false;
                        if (btnImg != null) btnImg.color = lockedBtnColor;
                        if (claimButtonText != null) claimButtonText.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}