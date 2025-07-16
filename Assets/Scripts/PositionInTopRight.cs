using UnityEngine;

public class PositionInTopRightCorner : MonoBehaviour
{
    void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogWarning("No parent found for " + gameObject.name);
            return;
        }

        // Try to get a RectTransform from the parent (for UI elements)
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        if (parentRect != null)
        {
            // Calculate the top-right corner in the parent's local space.
            // For a RectTransform, rect.xMax and rect.yMax give the local top-right.
            Vector2 topRight = new Vector2(parentRect.rect.xMax, parentRect.rect.yMax);

            // If this prefab also has a RectTransform (i.e. it is a UI element),
            // set its anchoredPosition; otherwise, use localPosition.
            RectTransform myRect = GetComponent<RectTransform>();
            if (myRect != null)
            {
                myRect.anchoredPosition = topRight;
            }
            else
            {
                // Use localPosition for non-UI elements.
                transform.localPosition = new Vector3(topRight.x, topRight.y, transform.localPosition.z);
            }
        }
        else
        {
            // If the parent doesn't have a RectTransform, try to use its Renderer bounds.
            Renderer parentRenderer = transform.parent.GetComponent<Renderer>();
            if (parentRenderer != null)
            {
                // Get the parent's top-right corner in world space.
                Vector3 topRightWorld = parentRenderer.bounds.max;
                // Convert the world position to the parent's local space.
                Vector3 topRightLocal = transform.parent.InverseTransformPoint(topRightWorld);
                transform.localPosition = new Vector3(topRightLocal.x, topRightLocal.y, transform.localPosition.z);
            }
            else
            {
                Debug.LogWarning("Parent does not have a RectTransform or Renderer component. Cannot determine top-right corner.");
            }
        }
    }
}
