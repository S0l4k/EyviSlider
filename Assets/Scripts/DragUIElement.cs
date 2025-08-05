using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragUIElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Drag Settings")]
    [Tooltip("Minimum alpha value to register clicks (0 = fully transparent, 1 = fully opaque)")]
    [Range(0, 1)] public float alphaThreshold = 0.1f;

    [Header("Visual Feedback")]
    [Tooltip("Scale multiplier when dragging (1 = no change)")]
    [SerializeField] private float dragScaleFactor = 1.05f;
    [Tooltip("Scaling animation speed")]
    [SerializeField] private float scaleSpeed = 10f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector2 offset;
    private Image image;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        originalScale = rectTransform.localScale;

        // Set up alpha-based click detection
        image.alphaHitTestMinimumThreshold = alphaThreshold;
    }

    private void Update()
    {
        // Smooth scaling animation
        if (rectTransform.localScale != targetScale)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            // Calculate offset from click position to object center
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            offset = rectTransform.position - worldPoint;
            isDragging = true;

            // Visual feedback
            targetScale = originalScale * dragScaleFactor;
            transform.SetAsLastSibling(); // Bring to front
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        targetScale = originalScale; // Return to normal scale
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && canvas.renderMode == RenderMode.WorldSpace)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            rectTransform.position = worldPoint + (Vector3)offset;
        }
    }
}