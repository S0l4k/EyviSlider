using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class WorldSpaceDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Drag Settings")]
    [Range(0, 1)] public float alphaThreshold = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private float dragScaleFactor = 1.05f;
    [SerializeField] private float scaleSpeed = 10f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector3 offset; // Changed to Vector3 to preserve Z
    private Image image;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private float originalZPosition; // Store original Z position

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
        originalZPosition = rectTransform.position.z; // Store initial Z position

        try
        {
            image.alphaHitTestMinimumThreshold = alphaThreshold;
        }
        catch (System.InvalidOperationException)
        {
            Debug.LogWarning("Alpha hit test disabled - texture not readable", this);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            // Store current Z position before drag starts
            originalZPosition = rectTransform.position.z;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            // Calculate offset while preserving original Z
            offset = rectTransform.position - worldPoint;
            offset.z = 0; // Only use X,Y for offset calculation

            isDragging = true;
            targetScale = originalScale * dragScaleFactor;

            // Bring to front using sorting order
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null) parentCanvas.sortingOrder = 1;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        targetScale = originalScale;
        PeruMinigamemanager PeruMain = Dependencies.Instance.GetDependancy<PeruMinigamemanager>();
        PeruMain.ChechDates();
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) parentCanvas.sortingOrder = 0;
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

            // Apply offset while maintaining original Z position
            worldPoint += (Vector3)offset;
            worldPoint.z = originalZPosition; // Preserve Z
            rectTransform.position = worldPoint;
        }
    }

    private void Update()
    {
        if (rectTransform.localScale != targetScale)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed);
        }
    }
}