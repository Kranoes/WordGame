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
        [SerializeField] private FpsSwitch fpsSwitch;

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

            // Автоматически подписываем события
            UnsubscribeUI();
            SubscribeUI();

            if (promoStatusText != null) promoStatusText.text = string.Empty;
        }

        private void OnDisable()
        {
            UnsubscribeUI();

            // Сохраняем на диск при закрытии панели
            SaveManager.Save();
        }

        private void SubscribeUI()
        {
            if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            if (uiSfxSlider != null) uiSfxSlider.onValueChanged.AddListener(OnUiSfxVolumeChanged);
            if (vibrationToggle != null) vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);

            if (fpsSwitch != null) fpsSwitch.OnFpsChanged += OnFpsSwitchChanged;
            if (closeButton != null) closeButton.onClick.AddListener(CloseWindow);
        }

        private void UnsubscribeUI()
        {
            if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            if (uiSfxSlider != null) uiSfxSlider.onValueChanged.RemoveListener(OnUiSfxVolumeChanged);
            if (vibrationToggle != null) vibrationToggle.onValueChanged.RemoveListener(OnVibrationToggled);

            if (fpsSwitch != null) fpsSwitch.OnFpsChanged -= OnFpsSwitchChanged;
            if (closeButton != null) closeButton.onClick.RemoveListener(CloseWindow);
        }

        private void LoadSettingsToUI()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            // Выставляем визуальное положение слайдеров без срабатывания ивентов
            if (musicSlider != null) musicSlider.SetValueWithoutNotify(data.musicVolume);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(data.sfxVolume);
            if (uiSfxSlider != null) uiSfxSlider.SetValueWithoutNotify(data.uiSfxVolume);

            if (vibrationToggle != null) vibrationToggle.SetIsOnWithoutNotify(data.isVibrationEnabled);

            if (fpsSwitch != null)
            {
                bool is60Fps = !data.isLowGraphics;
                fpsSwitch.SetState(is60Fps, notifyListeners: false);
            }

            ApplyGraphicsSettings(data.isLowGraphics);

            // Принудительно отдаем сохраненный звук в AudioManager
            AudioManager.Instance?.SetMusicVolume(data.musicVolume);
            AudioManager.Instance?.SetSfxVolume(data.sfxVolume);
            AudioManager.Instance?.SetUiSfxVolume(data.uiSfxVolume);
        }

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
                promoCodeInput.text = string.Empty;
                SaveManager.Save();
            }
        }

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

        public void CloseWindow()
        {
            AudioManager.Instance?.PlayUiSound();
            gameObject.SetActive(false);
        }
    }
}