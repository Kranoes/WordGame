using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WordleGame
{
    [RequireComponent(typeof(Button))]
    public class UIButtonSound : MonoBehaviour, IPointerClickHandler
    {
        [Header("Опционально: свой клик для этой кнопки")]
        [SerializeField] private AudioClip customClickSound;

        public void OnPointerClick(PointerEventData eventData)
        {
            Button btn = GetComponent<Button>();
            // Если кнопка неактивна (interactable = false), звук не играем
            if (btn != null && !btn.interactable) return;

            AudioManager.Instance?.PlayUiSound(customClickSound);
        }
    }
}