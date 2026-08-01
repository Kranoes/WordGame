using UnityEngine;
using UnityEngine.EventSystems;

namespace WordleGame
{
    public class UICursorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Cursor Settings")]
        [SerializeField] private Texture2D hoverCursor; // Текстура курсора при наведении
        [SerializeField] private Vector2 hotSpot = Vector2.zero; // Точка клика (активная точка курсора)

        // Срабатывает при наведении мыши на элемент
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverCursor != null)
            {
                Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
            }
        }

        // Срабатывает, когда мышь уходит с элемента
        public void OnPointerExit(PointerEventData eventData)
        {
            ResetCursor();
        }

        // Защита: если кнопка выключилась, пока мышь была на ней
        private void OnDisable()
        {
            ResetCursor();
        }

        private void ResetCursor()
        {
            // Сброс на дефолтный системный курсор
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}