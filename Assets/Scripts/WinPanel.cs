using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text targetWordText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private Button continueButton;

    public Button ContinueButton => continueButton;

    public void Setup(string secretWord, int rewardCoins, int currentStreak)
    {
        if (targetWordText != null) targetWordText.text = secretWord;
        if (rewardText != null) rewardText.text = $"+{rewardCoins}";
        if (streakText != null) streakText.text = currentStreak.ToString();
    }
}