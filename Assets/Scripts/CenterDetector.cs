using UnityEngine;

public class CenterDetector : MonoBehaviour
{
    [Tooltip("How close (in viewport units) the GameObject must be to the center (0.5, 0.5)")]
    public float centerThreshold = 0.05f;

    [Tooltip("Time (in seconds) the GameObject must remain in the center before playing the 'on' animation")]
    public float requiredTime = 1f;

    [Tooltip("Duration of the crossfade when transitioning to the 'off' animation")]
    public float exitTransitionDuration = 0.25f;

    public string buttonTag = "YourTagHere"; // Set the tag in the Inspector or assign it in code
    public string modalTag = "YourTagHere"; // Set the tag in the Inspector or assign it in code

    private float timer = 0f;
    private bool hasPlayedOn = false;
    private bool wasInCenter = false;

    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to this GameObject.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from " + gameObject.name);
        }
    }

    void Update()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No Main Camera found.");
            return;
        }

        // Convert the GameObject's world position to viewport coordinates.
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        bool currentlyInCenter = viewportPos.z > 0 &&
                                   Mathf.Abs(viewportPos.x - 0.5f) < centerThreshold &&
                                   Mathf.Abs(viewportPos.y - 0.5f) < centerThreshold;

        if (currentlyInCenter)
        {
            // Increment timer while staying in the center.
            timer += Time.deltaTime;

            // After required time in center, play the "on" animation if not already playing.
            if (timer >= requiredTime && !hasPlayedOn)
            {
                Debug.Log($"{gameObject.name} has been in the center for {requiredTime} second(s)!");
                animator.Play("on", 0, 0f);
                hasPlayedOn = true;
            }
        }
        else
        {
            // When leaving the center, if we were in the center last frame...
            if (hasPlayedOn)
            {
                Debug.Log($"{gameObject.name} has exited the center of the camera view!");

                // Get the current state's progress so the exit animation can start from there.
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
                float normalizedTimeOffset = currentState.normalizedTime % 1f;

                // CrossFade into the "off" animation, blending from the current state.
                animator.CrossFade("off", exitTransitionDuration, 0, normalizedTimeOffset);
                resetModals();
            }
            // Reset timer and flag when not in the center.
            timer = 0f;
            hasPlayedOn = false;
        }

        wasInCenter = currentlyInCenter;
    }

    public void resetModals()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag(buttonTag);
        foreach (GameObject obj in buttons)
        {
            BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.enabled = true; // Enable only the BoxCollider
            }
        }
        GameObject[] modals = GameObject.FindGameObjectsWithTag(modalTag);
        foreach (GameObject obj in modals)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
