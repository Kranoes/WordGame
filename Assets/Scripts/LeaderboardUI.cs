using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuessWordGame
{
    public class LeaderboardUI : MonoBehaviour
    {
        [Header("Контейнер элементов списка")]
        [SerializeField] private Transform contentContainer;
        [SerializeField] private GameObject itemPrefab;

        [Header("Кнопки вкладок")]
        [SerializeField] private Button winsTabButton;
        [SerializeField] private Button coinsTabButton;
        [SerializeField] private Button rubiesTabButton;
        [SerializeField] private Button streakTabButton;

        private LeaderboardCategory currentCategory = LeaderboardCategory.Wins;

        private void Start()
        {
            if (winsTabButton != null)
                winsTabButton.onClick.AddListener(() => SwitchCategory(LeaderboardCategory.Wins));

            if (coinsTabButton != null)
                coinsTabButton.onClick.AddListener(() => SwitchCategory(LeaderboardCategory.Coins));

            if (rubiesTabButton != null)
                rubiesTabButton.onClick.AddListener(() => SwitchCategory(LeaderboardCategory.Rubies));

            if (streakTabButton != null)
                streakTabButton.onClick.AddListener(() => SwitchCategory(LeaderboardCategory.Streak));
        }

        private void OnEnable()
        {
            RefreshUI();
        }

        public void SwitchCategory(LeaderboardCategory category)
        {
            currentCategory = category;
            RefreshUI();
        }

        public void RefreshUI()
        {
            if (contentContainer == null || itemPrefab == null)
            {
                Debug.LogWarning("LeaderboardUI: Не назначены contentContainer или itemPrefab в Инспекторе!");
                return;
            }

            // Очищаем старые строчки таблицы перед новой отрисовкой
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }

            if (LeaderboardManager.Instance == null)
            {
                Debug.LogError("LeaderboardUI: LeaderboardManager не найден на сцене!");
                return;
            }

            // Получаем отсортированный список лидеров
            List<LeaderboardEntry> leaderboard = LeaderboardManager.Instance.GetLeaderboard(currentCategory);

            // Заполняем таблицу элементами
            for (int i = 0; i < leaderboard.Count; i++)
            {
                GameObject obj = Instantiate(itemPrefab, contentContainer);
                LeaderboardItemUI item = obj.GetComponent<LeaderboardItemUI>();

                if (item != null)
                {
                    item.SetData(i + 1, leaderboard[i]);
                }
            }
        }
    }
}