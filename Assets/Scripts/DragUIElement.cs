using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragUIElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Drag Settings")]
    [Range(0, 1)] public float alphaThreshold = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private float dragScaleFactor = 1.05f;
    [SerializeField] private float scaleSpeed = 10f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector3 offset;
    private Image image;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private float originalZPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
        originalZPosition = rectTransform.position.z;

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
            originalZPosition = rectTransform.position.z;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            offset = rectTransform.position - worldPoint;
            offset.z = 0; 

            isDragging = true;
            targetScale = originalScale * dragScaleFactor;

            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null) parentCanvas.sortingOrder = 1;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        targetScale = originalScale;
        PuzzleComplitionCounter puzzleCounter = Dependencies.Instance.GetDependancy<PuzzleComplitionCounter>();
        puzzleCounter.CheckPuzzle();
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

            worldPoint += (Vector3)offset;
            worldPoint.z = originalZPosition;
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