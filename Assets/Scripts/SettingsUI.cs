using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordleGame
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Аудио (Sliders)")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider uiSfxSlider;

        [Header("Переключатели")]
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private FpsSwitch fpsSwitch; // Вместо старого graphicsToggle

        [Header("Промокоды")]
        [SerializeField] private TMP_InputField promoCodeInput;
        [SerializeField] private TextMeshProUGUI promoStatusText;

        [Header("Кнопка закрытия")]
        [SerializeField] private Button closeButton;

        [Header("Подтверждение сброса")]
        [SerializeField] private GameObject resetConfirmModal;

        private void OnEnable()
        {
            LoadSettingsToUI();

            if (promoStatusText != null) promoStatusText.text = "";

            // Подписка на события компонентов
            if (fpsSwitch != null) fpsSwitch.OnFpsChanged += OnFpsSwitchChanged;
            if (closeButton != null) closeButton.onClick.AddListener(CloseWindow);
        }

        private void OnDisable()
        {
            // Отписка от событий
            if (fpsSwitch != null) fpsSwitch.OnFpsChanged -= OnFpsSwitchChanged;
            if (closeButton != null) closeButton.onClick.RemoveListener(CloseWindow);

            // Сохраняем все изменения на диск ЕДИНОЖДЫ при закрытии окна
            SaveManager.Save();
        }

        private void LoadSettingsToUI()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            // Устанавливаем значения слайдеров без вызова OnValueChanged
            if (musicSlider != null) musicSlider.SetValueWithoutNotify(data.musicVolume);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(data.sfxVolume);
            if (uiSfxSlider != null) uiSfxSlider.SetValueWithoutNotify(data.uiSfxVolume);

            if (vibrationToggle != null) vibrationToggle.SetIsOnWithoutNotify(data.isVibrationEnabled);

            // Настройка состояния плашки 30 / 60 FPS (isLowGraphics = true значит 30 FPS)
            if (fpsSwitch != null)
            {
                bool is60Fps = !data.isLowGraphics;
                fpsSwitch.SetState(is60Fps, notifyListeners: false);
            }

            // Применяем настройки графики к движку при загрузке
            ApplyGraphicsSettings(data.isLowGraphics);
        }

        // --- ОБРАБОТЧИКИ ПОЛЗУНКОВ ---

        public void OnMusicVolumeChanged(float value)
        {
            if (SaveManager.CurrentData == null) return;
            SaveManager.CurrentData.musicVolume = value;
            AudioManager.Instance?.SetMusicVolume(value);
        }

        public void OnSfxVolumeChanged(float value)
        {
            if (SaveManager.CurrentData == null) return;
            SaveManager.CurrentData.sfxVolume = value;
            AudioManager.Instance?.SetSfxVolume(value);
        }

        public void OnUiSfxVolumeChanged(float value)
        {
            if (SaveManager.CurrentData == null) return;
            SaveManager.CurrentData.uiSfxVolume = value;
            AudioManager.Instance?.SetUiSfxVolume(value);
        }

        // --- ТУМБЛЕРЫ И ПЕРЕКЛЮЧАТЕЛИ ---

        public void OnVibrationToggled(bool value)
        {
            if (SaveManager.CurrentData == null) return;
            SaveManager.CurrentData.isVibrationEnabled = value;
        }

        private void OnFpsSwitchChanged(bool is60Fps)
        {
            if (SaveManager.CurrentData == null) return;

            bool isLowGraphics = !is60Fps;
            SaveManager.CurrentData.isLowGraphics = isLowGraphics;

            ApplyGraphicsSettings(isLowGraphics);
        }

        private void ApplyGraphicsSettings(bool isLow)
        {
            QualitySettings.SetQualityLevel(isLow ? 0 : 2, true);
            Application.targetFrameRate = isLow ? 30 : 60;
        }

        // --- ПРОМОКОДЫ ---

        public void OnSubmitPromoCode()
        {
            if (promoCodeInput == null) return;

            string code = promoCodeInput.text;
            bool success = PromoCodeManager.TryRedeemCode(code, out string message);

            if (promoStatusText != null)
            {
                promoStatusText.color = success ? Color.green : Color.red;
                promoStatusText.text = message;
            }

            if (success)
            {
                promoCodeInput.text = "";
                SaveManager.Save(); // Промокод дал награду — сохраняем сразу
            }
        }

        // --- СБРОС ПРОГРЕССА ---

        public void OnClickResetProgress()
        {
            if (resetConfirmModal != null) resetConfirmModal.SetActive(true);
        }

        public void ConfirmResetProgress()
        {
            SaveManager.ResetData();
            LoadSettingsToUI();
            AudioManager.Instance?.ApplyAllVolumes();

            if (resetConfirmModal != null) resetConfirmModal.SetActive(false);
            UIManager.Instance?.ShowInfoModal("СБРОС", "Весь прогресс успешно сброшен!");
        }

        // --- ЗАКРЫТИЕ ОКНА ---

        public void CloseWindow()
        {
            AudioManager.Instance?.PlayUiSound();
            gameObject.SetActive(false);
        }
    }
}