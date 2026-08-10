using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WordleGame
{
    public class DailyRewardPanelUI : MonoBehaviour
    {
        [Header("Массив из 7 карточек (День 1..7)")]
        [SerializeField] private DailyRewardCardUI[] dayCards;

        [Header("Кнопки и Тексты")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button claimButton;
        [SerializeField] private Button closeButton;

        private void OnEnable()
        {
            UpdateRewardView();

            if (claimButton != null) claimButton.onClick.AddListener(OnClaimClicked);
            if (closeButton != null) closeButton.onClick.AddListener(CloseWindow);
        }

        private void OnDisable()
        {
            if (claimButton != null) claimButton.onClick.RemoveListener(OnClaimClicked);
            if (closeButton != null) closeButton.onClick.RemoveListener(CloseWindow);
        }

        public void UpdateRewardView()
        {
            int currentDay = DailyRewardManager.GetCurrentStreakDay();
            bool canClaim = DailyRewardManager.CanClaimReward();
            bool isClaimedToday = !canClaim;

            // Перерисовываем все 7 карточек в соответствии с прогрессом
            for (int i = 0; i < dayCards.Length; i++)
            {
                if (dayCards[i] == null) continue;

                int dayNum = i + 1;
                DailyReward reward = DailyRewardManager.GetRewardForDay(dayNum);
                dayCards[i].Setup(dayNum, reward, currentDay, isClaimedToday);
            }

            if (claimButton != null)
            {
                claimButton.interactable = canClaim;
            }
        }

        private void OnClaimClicked()
        {
            if (DailyRewardManager.ClaimReward(out DailyReward reward))
            {
                AudioManager.Instance?.PlayUiSound();
                UpdateRewardView(); // Сразу перерисовываем карточки (текущая гаснет)
                CloseWindow();
            }
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
        }
    }
}