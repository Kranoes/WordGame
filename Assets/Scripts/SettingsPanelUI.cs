using UnityEngine;
using UnityEngine.UI;

namespace WordleGame
{
    public class SettingsPanelUI : MonoBehaviour
    {
        [Header("UI Слайдеры")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider uiSfxSlider;

        [Header("UI Чекбоксы и Переключатели")]
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private FpsSwitch fpsSwitch;

        private void OnEnable()
        {
            if (SettingsManager.Instance == null) return;

            // 1. При открытии панели запрашиваем текущие параметры из менеджера
            var settings = SettingsManager.Instance;

            UnsubscribeUI();

            // 2. Выставляем значения в UI
            if (musicSlider != null) musicSlider.value = settings.MusicVolume;
            if (sfxSlider != null) sfxSlider.value = settings.SfxVolume;
            if (uiSfxSlider != null) uiSfxSlider.value = settings.UiSfxVolume;
            if (vibrationToggle != null) vibrationToggle.isOn = settings.IsVibrationEnabled;
            if (fpsSwitch != null) fpsSwitch.SetState(settings.Is60Fps, false);

            SubscribeUI();
        }

        private void OnDisable()
        {
            UnsubscribeUI();

            // При закрытии окна сохраняем настройки на диск
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SaveSettings();
            }
        }

        private void SubscribeUI()
        {
            if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            if (uiSfxSlider != null) uiSfxSlider.onValueChanged.AddListener(OnUiSfxChanged);
            if (vibrationToggle != null) vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
            if (fpsSwitch != null) fpsSwitch.OnFpsChanged += OnFpsChanged;
        }

        private void UnsubscribeUI()
        {
            if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
            if (uiSfxSlider != null) uiSfxSlider.onValueChanged.RemoveListener(OnUiSfxChanged);
            if (vibrationToggle != null) vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
            if (fpsSwitch != null) fpsSwitch.OnFpsChanged -= OnFpsChanged;
        }

        private void OnMusicChanged(float val) => SettingsManager.Instance?.SetMusicVolume(val);
        private void OnSfxChanged(float val) => SettingsManager.Instance?.SetSfxVolume(val);
        private void OnUiSfxChanged(float val) => SettingsManager.Instance?.SetUiSfxVolume(val);
        private void OnVibrationChanged(bool val) => SettingsManager.Instance?.SetVibration(val);
        private void OnFpsChanged(bool is60) => SettingsManager.Instance?.SetFps(is60);
    }
}