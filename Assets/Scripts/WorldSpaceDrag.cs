using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class WorldSpaceDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector2 offset; // Stores click offset from object center

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            // Calculate offset between mouse click and object center
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            offset = rectTransform.position - worldPoint;
            isDragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        PeruMinigamemanager PeruMain = Dependencies.Instance.GetDependancy<PeruMinigamemanager>();
        PeruMain.ChechDates();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && canvas.renderMode == RenderMode.WorldSpace)
        {
            // Convert screen mouse position to world position
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            // Apply the offset we calculated when first clicked
            rectTransform.position = worldPoint + (Vector3)offset;
        }
    }
}