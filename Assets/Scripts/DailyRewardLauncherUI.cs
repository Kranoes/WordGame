using UnityEngine;
using UnityEngine.UI;

namespace WordleGame
{
    public class DailyRewardLauncherUI : MonoBehaviour
    {
        public static DailyRewardLauncherUI Instance { get; private set; }

        [SerializeField] private DailyRewardPanelUI rewardPanel;
        [SerializeField] private Button giftButton;
        [SerializeField] private Image exclamationBadge;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (giftButton != null)
            {
                giftButton.onClick.AddListener(OpenRewardPanel);
            }

            CheckAndAutoOpen();
        }

        public void CheckAndAutoOpen()
        {
            RewardManager.CheckAndUpdateStreak();
            UpdateBadge();

            if (RewardManager.CanClaimDailyReward() && rewardPanel != null)
            {
                rewardPanel.OpenPanel();
            }
        }

        public void UpdateBadge()
        {
            bool canClaim = RewardManager.CanClaimDailyReward();
            if (exclamationBadge != null)
            {
                exclamationBadge.gameObject.SetActive(canClaim);
            }
        }

        private void OpenRewardPanel()
        {
            if (rewardPanel != null)
            {
                rewardPanel.OpenPanel();
            }
        }
    }
}