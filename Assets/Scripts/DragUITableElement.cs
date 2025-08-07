using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragUITableElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Drag Settings")]
    [Range(0, 1)] public float alphaThreshold = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private float dragScaleFactor = 1.05f;
    [SerializeField] private float scaleSpeed = 10f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector2 offset;
    private Image image;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isInitialized = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();

        // Initialize scale values properly
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
        rectTransform.localScale = originalScale;

        image.alphaHitTestMinimumThreshold = alphaThreshold;
        isInitialized = true;
    }

    private void OnEnable()
    {
        // Reset scale when object is enabled
        if (isInitialized)
        {
            rectTransform.localScale = originalScale;
            targetScale = originalScale;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInitialized) return;

        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            offset = rectTransform.position - worldPoint;
            isDragging = true;
            targetScale = originalScale * dragScaleFactor;
            transform.SetAsLastSibling();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInitialized) return;

        isDragging = false;
        targetScale = originalScale;
        SacrificeTableScript tableScript = Dependencies.Instance.GetDependancy<SacrificeTableScript>();
        tableScript.ChechTable();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInitialized || !isDragging || canvas.renderMode != RenderMode.WorldSpace)
            return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint);

        rectTransform.position = worldPoint + (Vector3)offset;
    }
}