using UnityEngine;
using YG;

namespace GuessWordGame
{
    public static class SaveManager
    {
        private const string SAVE_KEY = "GuessWord_SaveData_v1";
        private static GameData _currentData;

        public static GameData CurrentData
        {
            get
            {
                if (_currentData == null)
                {
                    Load();
                }
                return _currentData;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            _currentData = null;
            YG2.onGetSDKData += OnYandexGetData;
            Load();
        }

        private static void OnYandexGetData()
        {
            if (YG2.saves != null && YG2.saves.gameData != null)
            {
                _currentData = YG2.saves.gameData;
            }
            else
            {
                Load();
            }
        }

        public static void Save()
        {
            if (_currentData == null) return;

            string json = JsonUtility.ToJson(_currentData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();

            if (YG2.isSDKEnabled && YG2.saves != null)
            {
                YG2.saves.gameData = _currentData;
                YG2.SaveProgress();
            }
        }

        public static void Load()
        {
            if (YG2.isSDKEnabled && YG2.saves != null && YG2.saves.gameData != null)
            {
                _currentData = YG2.saves.gameData;
                return;
            }

            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);

                try
                {
                    _currentData = JsonUtility.FromJson<GameData>(json);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[SaveManager] Ошибка чтения JSON: {ex.Message}");
                    _currentData = null;
                }
            }

            if (_currentData == null)
            {
                _currentData = new GameData();
            }
        }

        public static void ResetData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            _currentData = new GameData();

            if (YG2.isSDKEnabled && YG2.saves != null)
            {
                YG2.saves.gameData = _currentData;
                YG2.SaveProgress();
            }

            Save();
        }
    }
}