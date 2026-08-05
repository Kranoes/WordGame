using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordleGame
{
    public class FpsSwitch : MonoBehaviour
    {
        [Header("Кнопки UI")]
        [SerializeField] private Button button30;
        [SerializeField] private Button button60;

        [Header("Фон кнопок (Image)")]
        [SerializeField] private Image bg30;
        [SerializeField] private Image bg60;

        [Header("Текст кнопок")]
        [SerializeField] private TextMeshProUGUI text30;
        [SerializeField] private TextMeshProUGUI text60;

        [Header("Цвета фонов")]
        [SerializeField] private Color activeBgColor = new Color(0.2f, 0.6f, 1f, 1f);   // Активная кнопка
        [SerializeField] private Color inactiveBgColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Неактивная кнопка

        [Header("Цвета текста")]
        [SerializeField] private Color activeTextColor = Color.white;
        [SerializeField] private Color inactiveTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        // Action<bool>: true = 60 FPS, false = 30 FPS
        public event Action<bool> OnFpsChanged;
        public bool Is60Fps { get; private set; }

        private void Awake()
        {
            if (button30 != null) button30.onClick.AddListener(() => SetState(false, true));
            if (button60 != null) button60.onClick.AddListener(() => SetState(true, true));
        }

        public void SetState(bool is60Fps, bool notifyListeners)
        {
            Is60Fps = is60Fps;

            // Обновляем цвета фонов
            if (bg30 != null) bg30.color = !is60Fps ? activeBgColor : inactiveBgColor;
            if (bg60 != null) bg60.color = is60Fps ? activeBgColor : inactiveBgColor;

            // Обновляем цвета текста
            if (text30 != null) text30.color = !is60Fps ? activeTextColor : inactiveTextColor;
            if (text60 != null) text60.color = is60Fps ? activeTextColor : inactiveTextColor;

            if (notifyListeners)
            {
                OnFpsChanged?.Invoke(Is60Fps);
            }
        }
    }
}