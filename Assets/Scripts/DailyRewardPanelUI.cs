using UnityEngine;
using UnityEngine.UI;

namespace WordleGame
{
    public class DailyRewardPanelUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private DailyRewardCardUI[] cards;
        [SerializeField] private Button closeButton;

        [Header("Audio FX")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip coinSound;
        [SerializeField] private AudioClip rubySound;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void OnEnable()
        {
            RefreshPanel();
        }

        public void OpenPanel()
        {
            gameObject.SetActive(true);
            RefreshPanel();
        }

        public void RefreshPanel()
        {
            if (SaveManager.CurrentData == null) return;

            int currentStreak = SaveManager.CurrentData.loginStreak;
            bool canClaim = RewardManager.CanClaimDailyReward();

            for (int i = 0; i < cards.Length; i++)
            {
                int dayNumber = i + 1;
                RewardState state;

                if (i < currentStreak)
                {
                    state = RewardState.Claimed;
                }
                else if (i == currentStreak && canClaim)
                {
                    state = RewardState.ReadyToClaim;
                }
                else
                {
                    state = RewardState.Locked;
                }

                var (amount, isRuby) = RewardManager.GetDailyRewardConfig(i);
                cards[i].Setup(dayNumber, amount, isRuby, state, OnClaimButtonClicked);
            }

            DailyRewardLauncherUI.Instance?.UpdateBadge();
        }

        private void OnClaimButtonClicked()
        {
            if (SaveManager.CurrentData == null) return;

            int currentStreak = SaveManager.CurrentData.loginStreak;
            var (_, isRuby) = RewardManager.GetDailyRewardConfig(currentStreak);

            if (RewardManager.ClaimDailyReward())
            {
                PlayRewardSound(isRuby);
                RefreshPanel();
            }
        }

        private void PlayRewardSound(bool isRuby)
        {
            AudioClip clipToPlay = isRuby ? rubySound : coinSound;

            if (audioSource != null && clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }
        }

        private void ClosePanel()
        {
            gameObject.SetActive(false);
            DailyRewardLauncherUI.Instance?.UpdateBadge();
        }
    }
}