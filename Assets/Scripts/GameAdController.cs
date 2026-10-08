using UnityEngine;
using UnityEngine.Events;
using YG;

namespace GuessWordGame
{
    public class GameAdController : MonoBehaviour
    {
        private const string REWARD_EXTRA_ATTEMPT = "extra_attempt";

        [SerializeField] private UnityEvent onExtraAttemptGranted;

        private void OnEnable()
        {
            YG2.onRewardAdv += OnRewardReceived;
        }

        private void OnDisable()
        {
            YG2.onRewardAdv -= OnRewardReceived;
        }

        public void ShowExtraAttemptAd()
        {
            YG2.RewardedAdvShow(REWARD_EXTRA_ATTEMPT);
        }

        public void ShowInterstitialAd()
        {
            YG2.InterstitialAdvShow();
        }

        private void OnRewardReceived(string id)
        {
            if (id == REWARD_EXTRA_ATTEMPT)
            {
                onExtraAttemptGranted?.Invoke();
            }
        }
    }
}