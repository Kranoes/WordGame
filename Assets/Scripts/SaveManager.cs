using UnityEngine;

namespace WordleGame
{
    public static class SaveManager
    {
        private const string SaveKey = "Wordle_Player_Save";

        // Глобальный доступ к данным из любого скрипта игры
        public static GameData CurrentData { get; private set; }

        // Вызывать один раз при самом старте игры (например, в Awake вашего GameManager)
        public static void Initialize()
        {
            Load();
        }

        // Сохранить текущее состояние
        public static void Save()
        {
            if (CurrentData == null) return;

            // Переводим объект с данными в JSON-строку и сохраняем
            string json = JsonUtility.ToJson(CurrentData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        // Загрузить данные из памяти
        public static void Load()
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                string json = PlayerPrefs.GetString(SaveKey);
                CurrentData = JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                // Если игры ещё не было — создаём абсолютно чистый профиль
                CurrentData = new GameData();
                Save();
            }
        }

        // Метод для тестов (чтобы сбросить прогресс одной кнопкой)
        public static void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            CurrentData = new GameData();
            Save();
        }
    }
}