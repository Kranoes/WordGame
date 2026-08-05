using UnityEngine;

namespace WordleGame
{
    public static class SaveManager
    {
        private const string SAVE_KEY = "Wordle_SaveData_v1";
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

        public static void Initialize()
        {
            Load();
        }

        public static void Save()
        {
            if (_currentData == null) return;

            // 1. Сериализуем объект C# в JSON-строку
            string json = JsonUtility.ToJson(_currentData);

            // 2. Записываем в localStorage браузера через PlayerPrefs
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public static void Load()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);

                try
                {
                    // 3. Десериализуем JSON-строку обратно в поле _currentData
                    _currentData = JsonUtility.FromJson<GameData>(json);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[SaveManager] Ошибка чтения JSON: {ex.Message}. Данные пересозданы.");
                    _currentData = null;
                }
            }

            // Если сохранения нет, произошел сбой или FromJson вернул null
            if (_currentData == null)
            {
                _currentData = new GameData();
            }
        }

        public static void ResetData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            _currentData = new GameData();
            Save();
        }
    }
}