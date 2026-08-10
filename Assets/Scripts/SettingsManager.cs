using UnityEngine;

namespace WordleGame
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        private const string MusicKey = "Settings_MusicVolume";
        private const string SfxKey = "Settings_SfxVolume";
        private const string UiSfxKey = "Settings_UiSfxVolume";
        private const string FpsKey = "Settings_Is60Fps";
        private const string VibrationKey = "Settings_VibrationEnabled";

        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
        public float UiSfxVolume { get; private set; }
        public bool Is60Fps { get; private set; }
        public bool IsVibrationEnabled { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            LoadSettings();
        }

        private void Start()
        {
            // Вызываем применение настроек в Start, когда AudioManager уже гарантированно инициализирован в Awake
            ApplySettings();
        }

        public void LoadSettings()
        {
            // Второй аргумент — это значение по умолчанию, если ключ ещё не был сохранен
            MusicVolume = PlayerPrefs.GetFloat(MusicKey, 0.5f);
            SfxVolume = PlayerPrefs.GetFloat(SfxKey, 0.5f);
            UiSfxVolume = PlayerPrefs.GetFloat(UiSfxKey, 0.5f);

            Is60Fps = PlayerPrefs.GetInt(FpsKey, 1) == 1;
            IsVibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) == 1;
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(MusicKey, MusicVolume);
            PlayerPrefs.SetFloat(SfxKey, SfxVolume);
            PlayerPrefs.SetFloat(UiSfxKey, UiSfxVolume);
            PlayerPrefs.SetInt(FpsKey, Is60Fps ? 1 : 0);
            PlayerPrefs.SetInt(VibrationKey, IsVibrationEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ApplySettings()
        {
            Application.targetFrameRate = Is60Fps ? 60 : 30;

            SetMusicVolume(MusicVolume);
            SetSfxVolume(SfxVolume);
            SetUiSfxVolume(UiSfxVolume);
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = value;
            if (AudioManager.Instance != null && AudioManager.Instance.MusicSource != null)
            {
                AudioManager.Instance.MusicSource.volume = value;
            }
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = value;
            if (AudioManager.Instance != null && AudioManager.Instance.SfxSource != null)
            {
                AudioManager.Instance.SfxSource.volume = value;
            }
        }

        public void SetUiSfxVolume(float value)
        {
            UiSfxVolume = value;
            if (AudioManager.Instance != null && AudioManager.Instance.UiSfxSource != null)
            {
                AudioManager.Instance.UiSfxSource.volume = value;
            }
        }

        public void SetVibration(bool enabled) => IsVibrationEnabled = enabled;

        public void SetFps(bool is60)
        {
            Is60Fps = is60;
            Application.targetFrameRate = is60 ? 60 : 30;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveSettings();
        }

        private void OnApplicationQuit() => SaveSettings();
    }
}