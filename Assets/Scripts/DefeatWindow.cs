using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;

namespace GuessWordGame
{
    public class DefeatWindow : MonoBehaviour
    {
        private const string REWARD_SAVE_STREAK = "save_streak";
        private const string REWARD_EXTRA_ATTEMPT = "extra_attempt";

        [Header("Buttons")]
        [SerializeField] private Button extraAttemptButton;
        [SerializeField] private Button saveStreakButton;
        [SerializeField] private TextMeshProUGUI extraAttemptText;
        [SerializeField] private TextMeshProUGUI streakText;

        private void OnEnable()
        {
            YG2.onRewardAdv += OnRewardReceived;
            RefreshWindowData();
        }

        private void OnDisable()
        {
            YG2.onRewardAdv -= OnRewardReceived;
        }

        public void RefreshWindowData()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            if (streakText != null)
            {
                streakText.text = $"Текущая серия: {data.currentWinStreak}";
            }

            if (data.extraAttemptHints > 0)
            {
                extraAttemptText.text = $"Взять +1 шанс (есть {data.extraAttemptHints})";
            }
            else
            {
                extraAttemptText.text = "Взять +1 шанс (Реклама)";
            }
        }

        public void OnClickExtraAttempt()
        {
            GameData data = SaveManager.CurrentData;
            if (data != null && data.extraAttemptHints > 0)
            {
                data.extraAttemptHints--;
                SaveManager.Save();
                GrantExtraAttempt();
            }
            else
            {
                YG2.RewardedAdvShow(REWARD_EXTRA_ATTEMPT);
            }
        }

        public void OnClickSaveStreak()
        {
            YG2.RewardedAdvShow(REWARD_SAVE_STREAK);
        }

        public void OnClickAcceptDefeat()
        {
            GameData data = SaveManager.CurrentData;
            if (data != null)
            {
                data.currentWinStreak = 0;
                SaveManager.Save();
            }

            YG2.InterstitialAdvShow();
            gameObject.SetActive(false);

        }

        private void OnRewardReceived(string id)
        {
            if (id == REWARD_EXTRA_ATTEMPT)
            {
                GrantExtraAttempt();
            }
            else if (id == REWARD_SAVE_STREAK)
            {
                SaveStreakAndExit();
            }
        }

        private void GrantExtraAttempt()
        {
            gameObject.SetActive(false);

        }

        private void SaveStreakAndExit()
        {

            YG2.InterstitialAdvShow();
            gameObject.SetActive(false);
        }
    }
}