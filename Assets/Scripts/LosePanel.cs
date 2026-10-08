using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LosePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text targetWordText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private Button continueButton;

    public Button ContinueButton => continueButton;

    public void Setup(string secretWord)
    {
        if (targetWordText != null) targetWordText.text = secretWord;
        if (rewardText != null) rewardText.text = "+0";
        if (streakText != null) streakText.text = "0";
    }
}