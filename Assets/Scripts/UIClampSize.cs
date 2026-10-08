using UnityEngine;

namespace GuessWordGame
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class UIClampSize : MonoBehaviour
    {
        [Header("Ограничения (в пикселях)")]
        [SerializeField] private Vector2 minSize = new Vector2(320, 450);
        [SerializeField] private Vector2 maxSize = new Vector2(700, 850);

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ApplyClamps();
        }

        private void OnRectTransformDimensionsChange()
        {
            ApplyClamps();
        }

        public void ApplyClamps()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            Vector2 currentSize = _rectTransform.rect.size;

            float clampedWidth = Mathf.Clamp(currentSize.x, minSize.x, maxSize.x);
            float clampedHeight = Mathf.Clamp(currentSize.y, minSize.y, maxSize.y);

            if (Mathf.Approximately(currentSize.x, clampedWidth) && Mathf.Approximately(currentSize.y, clampedHeight))
                return;

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clampedWidth);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clampedHeight);
        }
    }
}