using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;

namespace GuessWordGame
{
    public class HintButtonUI : MonoBehaviour
    {
        public enum HintType
        {
            ExtraAttempt,
            RevealLetter,
            RemoveLetter
        }

        [SerializeField] private HintType hintType;
        [SerializeField] private int priceInCoins = 100;

        [Header("UI Elements")]
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private GameObject adIcon;

        private void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            int count = GetHintCount(data);

            if (count > 0)
            {
                if (adIcon != null) adIcon.SetActive(false);
                buttonText.text = $"x{count}";
            }
            else
            {
                if (adIcon != null) adIcon.SetActive(true);
                buttonText.text = $"{priceInCoins}";
            }
        }

        public void OnClick()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            int count = GetHintCount(data);

            if (count > 0)
            {
                ConsumeHint(data);
                SaveManager.Save();
                UpdateUI();
                ApplyHintEffect();
            }
            else if (data.coins >= priceInCoins)
            {
                data.coins -= priceInCoins;
                SaveManager.Save();
                UIManager.Instance?.UpdateCurrencyUI();
                UpdateUI();
                ApplyHintEffect();
            }
            else
            {
                YG2.RewardedAdvShow($"buy_hint_{hintType}");
            }
        }

        private int GetHintCount(GameData data)
        {
            return hintType switch
            {
                HintType.ExtraAttempt => data.extraAttemptHints,
                HintType.RevealLetter => data.revealLetterHints,
                HintType.RemoveLetter => data.removeLetterHints,
                _ => 0
            };
        }

        private void ConsumeHint(GameData data)
        {
            switch (hintType)
            {
                case HintType.ExtraAttempt: data.extraAttemptHints--; break;
                case HintType.RevealLetter: data.revealLetterHints--; break;
                case HintType.RemoveLetter: data.removeLetterHints--; break;
            }
        }

        private void ApplyHintEffect()
        {

        }
    }
}