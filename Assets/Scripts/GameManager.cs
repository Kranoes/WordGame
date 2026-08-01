using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WordleGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private KeyboardManager keyboardManager;

        [Header("Game State")]
        private GameMode currentMode = GameMode.FreePlay;
        public GameMode CurrentMode => currentMode;

        private string secretWord;
        private readonly Cell[,] cells = new Cell[6, 5];
        private int currentRow = 0;
        private int currentCol = 0;
        private int attempts = 0;
        private int wins = 0;

        private const int MaxAttempts = 6;
        private const int WordLength = 5;

        private bool gameActive = false;
        private bool isInputBlocked = false;

        private readonly List<string> wordPool = new List<string>();
        public bool IsGameActive => gameActive;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            LoadWordsFromFile();
            SaveManager.Initialize();
        }

        private void Start()
        {
            gameActive = false;

            wins = SaveManager.CurrentData.totalWins;

            RewardManager.CheckAndGrantDailyLogin();
            UpdateScoreUI();

            // Принудительно включаем главное меню при старте сцены
            UIManager.Instance?.ShowMainMenu(true);
        }

        private void Update()
        {
            if (!gameActive || isInputBlocked) return;

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                EraseLastLetter();
                return;
            }

            if (Input.anyKeyDown)
            {
                foreach (char c in Input.inputString)
                {
                    char upperChar = char.ToUpper(c);
                    if ((upperChar >= 'А' && upperChar <= 'Я') || (upperChar >= 'A' && upperChar <= 'Z'))
                    {
                        OnLetterPressed(upperChar);
                        return;
                    }
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            AudioListener.pause = !hasFocus;
            Time.timeScale = hasFocus ? 1f : 0f;
        }

        public void ReturnToMenu()
        {
            gameActive = false;
            isInputBlocked = false;

            UIManager.Instance?.ClearStatusText();
            UIManager.Instance?.HideResultModal();
            UIManager.Instance?.ShowMainMenu(true);
        }

        public bool RemoveIncorrectKeyboardLetters(int count)
        {
            if (string.IsNullOrEmpty(secretWord) || !gameActive) return false;

            if (KeyboardManager.Instance != null)
            {
                return KeyboardManager.Instance.DisableRandomIncorrectKeys(secretWord, count);
            }

            return false;
        }

        public bool RevealRandomLetter()
        {
            if (string.IsNullOrEmpty(secretWord) || !gameActive) return false;

            string upperSecret = secretWord.ToUpper();

            // Проверяем, есть ли еще свободная позиция в текущей строке
            if (currentCol < WordLength && currentCol < upperSecret.Length)
            {
                char correctLetter = upperSecret[currentCol];

                // Имитируем нажатие правильной буквы
                OnLetterPressed(correctLetter);
                return true;
            }

            return false;
        }

        public void StartGameWithMode(GameMode mode)
        {
            currentMode = mode;

            // Обновляем видимость кнопок подсказок
            HintManager.Instance?.UpdateHintButtonsVisibility(currentMode);

            if (currentMode == GameMode.Daily)
            {
                string today = System.DateTime.UtcNow.ToString("yyyy-MM-dd");
                if (SaveManager.CurrentData.lastDailyCaseDate == today)
                {
                    UIManager.Instance?.ShowInfoModal("ЕЖЕДНЕВНЫЙ РЕЖИМ", "Вы уже отгадали сегодняшнее слово!\nВозвращайтесь завтра.");
                    gameActive = false;
                    return;
                }
            }

            UIManager.Instance?.ShowMainMenu(false);
            UIManager.Instance?.HideResultModal();
            ResetGame();
        }

        public void StartDailyGame()
        {
            StartGameWithMode(GameMode.Daily);
        }

        public void StartFreePlayGame()
        {
            StartGameWithMode(GameMode.FreePlay);
        }

        public void RegisterCell(Cell cell, int row, int col)
        {
            cells[row, col] = cell;
        }

        public void OnBoardCreated()
        {
            isInputBlocked = false;
        }

        public void OnLetterPressed(char letter)
        {
            if (!gameActive || isInputBlocked) return;
            if (currentRow >= MaxAttempts || currentCol >= WordLength) return;

            cells[currentRow, currentCol].SetLetter(letter);
            cells[currentRow, currentCol].Highlight(false);

            currentCol++;

            if (currentCol < WordLength)
            {
                cells[currentRow, currentCol].Highlight(true);
            }
            else
            {
                StartCoroutine(CheckWordRoutine());
            }
        }

        public void EraseLastLetter()
        {
            if (!gameActive || isInputBlocked) return;

            if (currentCol > 0)
            {
                if (currentCol < WordLength)
                    cells[currentRow, currentCol].Highlight(false);

                currentCol--;
                cells[currentRow, currentCol].SetLetter(' ');
                cells[currentRow, currentCol].Highlight(true);
            }
        }
        private void RecordGameResult(bool isWin, int attemptsCount)
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            // При ЗАВЕРШЕНИИ ЛЮБОЙ ИГРЫ (и победа, и поражение):
            SaveManager.CurrentData.gamesPlayed++;

            if (isWin)
            {
                SaveManager.CurrentData.totalWins++;
                SaveManager.CurrentData.currentWinStreak++;

                // Обновляем максимальную серию
                if (SaveManager.CurrentData.currentWinStreak > SaveManager.CurrentData.maxWinStreak)
                {
                    SaveManager.CurrentData.maxWinStreak = SaveManager.CurrentData.currentWinStreak;
                }

                // Записываем попытку в гистограмму (attemptsCount - от 1 до 6)
                int attemptIndex = attemptsCount - 1; // ИСПРАВЛЕНО: с currentAttempt на attemptsCount
                if (attemptIndex >= 0 && attemptIndex < 6)
                {
                    SaveManager.CurrentData.guessDistribution[attemptIndex]++;
                }
            }
            else // Поражение
            {
                SaveManager.CurrentData.currentWinStreak = 0; // Сбрасываем текущую серию
            }

            // ОБЯЗАТЕЛЬНО сохраняем изменения на диск!
            SaveManager.Save();
        }
        private void LoadWordsFromFile()
        {
            TextAsset wordsFile = Resources.Load<TextAsset>("words");
            wordPool.Clear();

            if (wordsFile != null)
            {
                string[] lines = wordsFile.text.Split('\n');
                foreach (string line in lines)
                {
                    string word = line.Trim().ToUpper();
                    if (word.Length == WordLength && !string.IsNullOrEmpty(word))
                        wordPool.Add(word);
                }
            }

            if (wordPool.Count == 0)
            {
                Debug.LogError("Словарь пуст или не найден! Загружены резервные слова.");
                wordPool.AddRange(new string[] { "ПРИВЕТ", "СЛОВО", "ГОРОД", "ОКЕАН", "ПЛАЗА" });
            }
        }

        private IEnumerator CheckWordRoutine()
        {
            isInputBlocked = true;

            string word = "";
            for (int i = 0; i < WordLength; i++)
                word += cells[currentRow, i].GetLetter();

            // 1. ОШИБКА: Слова нет в словаре
            if (!wordPool.Contains(word))
            {
                Coroutine[] shakeCoroutines = new Coroutine[WordLength];
                for (int i = 0; i < WordLength; i++)
                {
                    shakeCoroutines[i] = StartCoroutine(cells[currentRow, i].AnimateShakeRoutine());
                }

                yield return new WaitForSeconds(0.52f);

                for (int i = 0; i < WordLength; i++)
                {
                    cells[currentRow, i].ResetToDefault();
                }
                currentCol = 0;
                cells[currentRow, 0].Highlight(true);
                isInputBlocked = false;
                yield break;
            }

            // 2. УСПЕШНАЯ ПРОВЕРКА: Поочередный переворот ячеек
            attempts++;
            LetterState[] states = WordChecker.EvaluateWord(word, secretWord);

            for (int i = 0; i < WordLength; i++)
            {
                StartCoroutine(cells[currentRow, i].AnimateFlipRoutine(states[i], 0f));
                keyboardManager?.UpdateKeyStatus(word[i], states[i]);
                yield return new WaitForSeconds(0.23f);
            }

            yield return new WaitForSeconds(0.30f);

            // Проверяем победу
            // Проверяем победу
            bool isWin = true;
            foreach (var state in states)
            {
                if (state != LetterState.Green) isWin = false;
            }

            if (isWin)
            {
                // Записываем статистику победы
                RecordGameResult(true, attempts);

                string rewardMessage = "";
                if (currentMode == GameMode.Daily)
                {
                    RewardManager.GrantDailyWinReward(attempts, out int droppedCoins);
                    rewardMessage = $"+{droppedCoins} монет!";
                }
                else
                {
                    if (RewardManager.TryGrantFreePlayReward(attempts, out int droppedCoins))
                        rewardMessage = $"+{droppedCoins} монет! ({SaveManager.CurrentData.freeCasesOpenedToday}/5)";
                    else
                        rewardMessage = "Лимит кейсов на сегодня исчерпан";
                }

                UpdateScoreUI();

                // Показываем модальное окно победы и передаем актуальный currentWinStreak
                gameActive = false;
                UIManager.Instance?.ShowWinModal(rewardMessage, SaveManager.CurrentData.currentWinStreak);
            }
            else if (attempts >= MaxAttempts)
            {
                // Записываем статистику поражения
                RecordGameResult(false, attempts);

                // Показываем модальное окно поражения и замораживаем ввод
                gameActive = false;
                UIManager.Instance?.ShowLoseModal(secretWord);
            }
            else
            {
                // Переход на новую строку
                currentRow++;
                currentCol = 0;
                cells[currentRow, 0].Highlight(true);
                isInputBlocked = false;
            }
        }

        private void ChooseNewSecretWord()
        {
            if (wordPool == null || wordPool.Count == 0)
            {
                Debug.LogError("[GameManager] Невозможно выбрать слово: словарь пуст!");
                return;
            }

            if (currentMode == GameMode.Daily)
            {
                // Формируем численный seed на основе текущей даты в UTC
                string dateStr = System.DateTime.UtcNow.ToString("yyyyMMdd");
                int dateSeed = int.Parse(dateStr);

                // Используем локальный System.Random — он изолирован и не сбивает глобальный рандом Unity
                System.Random dailyRandom = new System.Random(dateSeed);
                secretWord = wordPool[dailyRandom.Next(0, wordPool.Count)];
            }
            else
            {
                // Для обычного режима используем стандартный случайный выбор Unity
                secretWord = wordPool[UnityEngine.Random.Range(0, wordPool.Count)];
            }

            Debug.Log($"[GameManager] Режим: {currentMode}. Загаданное слово: {secretWord}");
        }
        /// <summary>
        /// Отменяет последнюю потраченную попытку и очищает строку
        /// </summary>
        public bool GrantExtraAttempt()
        {
            if (attempts <= 0) return false;

            // Если раунд завершился поражением (gameActive == false), сбрасываем текущую строчку currentRow.
            // Если раунд еще идет (gameActive == true), currentRow указывает на новую пустую строку, поэтому сбрасываем currentRow - 1.
            int targetRow = !gameActive ? (currentRow < MaxAttempts ? currentRow : MaxAttempts - 1)
                                        : (currentRow > 0 ? currentRow - 1 : 0);

            for (int c = 0; c < WordLength; c++)
            {
                cells[targetRow, c]?.ResetToDefault();
            }

            currentRow = targetRow;
            currentCol = 0;
            if (attempts > 0) attempts--;

            gameActive = true;
            isInputBlocked = false;

            cells[currentRow, 0]?.Highlight(true);
            UIManager.Instance?.HideResultModal();

            return true;
        }
        public void ResetGame()
        {
            currentRow = 0;
            currentCol = 0;
            attempts = 0;

            for (int r = 0; r < MaxAttempts; r++)
            {
                for (int c = 0; c < WordLength; c++)
                {
                    cells[r, c]?.ResetToDefault();
                }
            }

            keyboardManager?.ResetKeyboard();
            ChooseNewSecretWord();
            UpdateScoreUI();

            // Скрываем окно результатов при начале нового раунда
            UIManager.Instance?.HideResultModal();

            isInputBlocked = false;
            gameActive = true;
            cells[0, 0]?.Highlight(true);
            UIManager.Instance?.ClearStatusText();
        }

        private void UpdateScoreUI()
        {
            if (debugText != null)
            {
                debugText.text = $"Побед: {wins}";
            }
        }
    }
}