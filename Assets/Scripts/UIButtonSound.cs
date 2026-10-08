using UnityEngine;
using UnityEngine.UI;

namespace GuessWordGame
{
    [RequireComponent(typeof(Button))]
    public class UIButtonSound : MonoBehaviour
    {
        [Header("Опционально: свой клик для этой кнопки")]
        [SerializeField] private AudioClip customClickSound;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(PlaySound);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(PlaySound);
            }
        }

        private void PlaySound()
        {
            // button.interactable проверяется движком Unity перед вызовом onClick автоматически
            AudioManager.Instance?.PlayUiSound(customClickSound);
        }
    }
}