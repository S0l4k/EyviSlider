using UnityEngine;
using System.Collections;

public class ClickActivatorWithFade : MonoBehaviour
{
    [Header("Target Settings")]
    // The GameObject to activate and fade in when this object is clicked.
    public GameObject targetObject;

    [Header("Fade Settings")]
    // Duration of the fade in (in seconds).
    public float fadeDuration = 1f;
    public string targetTag = "YourTagHere"; // Set the tag in the Inspector or assign it in code


    // private bool isActivated = false;

    // This method is called when the object is clicked.
    // (Make sure the GameObject has a Collider component attached.)
    void OnMouseDown()
    {
        if (targetObject != null)
        {
            // isActivated = true;
            // Activate the target object.
            targetObject.SetActive(true);
            DisableAllBoxColliders();

            // Get or add a CanvasGroup component on the target.
            CanvasGroup canvasGroup = targetObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = targetObject.AddComponent<CanvasGroup>();
            }
            // Start the target fully transparent.
            canvasGroup.alpha = 0f;

            // Start the fade-in coroutine.
            StartCoroutine(FadeIn(canvasGroup));
        }
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            // Update the alpha value based on elapsed time.
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
    public void DisableAllBoxColliders()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in objects)
        {
            BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false; // Disable only the BoxCollider
            }
        }
    }

}
