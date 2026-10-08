using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevivePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text streakPreviewText;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button buyExtraRowButton;
    [SerializeField] private Button giveUpButton;

    [Header("Dynamic Button Elements")]
    [SerializeField] private Image extraRowIcon;
    [SerializeField] private TMP_Text extraRowText;
    [SerializeField] private Sprite hintSprite;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite adSprite;

    public Button WatchAdButton => watchAdButton;
    public Button BuyExtraRowButton => buyExtraRowButton;
    public Button GiveUpButton => giveUpButton;

    public void Setup(int currentStreak, int hintInventoryCount, int playerCoins, int hintCoinCost)
    {
        if (streakPreviewText != null)
        {
            streakPreviewText.text = $"Серия на кону: {currentStreak}";
        }

        ConfigureExtraRowButton(hintInventoryCount, playerCoins, hintCoinCost);
    }

    private void ConfigureExtraRowButton(int hints, int coins, int cost)
    {
        if (hints > 0)
        {
            extraRowIcon.sprite = hintSprite;
            extraRowText.text = $"Использовать ({hints})";
            buyExtraRowButton.interactable = true;
        }
        else if (coins >= cost)
        {
            extraRowIcon.sprite = coinSprite;
            extraRowText.text = cost.ToString();
            buyExtraRowButton.interactable = true;
        }
        else
        {
            extraRowIcon.sprite = adSprite;
            extraRowText.text = "+1 Ряд";
            buyExtraRowButton.interactable = true;
        }
    }
}