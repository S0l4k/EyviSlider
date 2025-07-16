using UnityEngine;
using System.Collections;
using UnityEngine.Video; // Needed for VideoPlayer

public class IdleFadeActivator : MonoBehaviour
{
    [Header("Idle Settings")]
    [Tooltip("Time (in seconds) of no interaction before activating the target object.")]
    public float idleTime = 5.0f;

    [Header("Fade Settings")]
    [Tooltip("Duration (in seconds) for the fade in animation.")]
    public float fadeInDuration = 1.0f;
    [Tooltip("Duration (in seconds) for the fade out animation.")]
    public float fadeOutDuration = 1.0f;
    [Tooltip("Target opacity (alpha) when fully visible (0 = transparent, 1 = opaque).")]
    [Range(0, 1)]
    public float targetAlpha = 1.0f;

    [Header("Interaction Settings")]
    [Tooltip("Minimum camera movement (in world units) to count as interaction.")]
    public float cameraMovementThreshold = 0.01f;

    [Header("Target Object")]
    [Tooltip("The parent GameObject whose children (with SpriteRenderers or VideoPlayers) will fade in/out.")]
    public GameObject targetObject;

    // Idle timer
    private float timer = 0f;
    private Vector3 lastCameraPosition;
    private Coroutine fadeCoroutine;

    private const string KEY_IDLE_TIME = "IdleTime";
    private const string KEY_FADE_IN = "FadeIn";
    private const string KEY_FADE_OUT = "FadeOut";

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object not assigned.");
            return;
        }
        // Ensure the target object is initially inactive.
        targetObject.SetActive(false);
        // Set all child SpriteRenderers and VideoPlayers to be fully transparent.
        SetChildrenAlpha(0f);

        // Cache main camera position.
        if (Camera.main != null)
        {
            lastCameraPosition = Camera.main.transform.position;
        }

        // Load saved settings.
        LoadSettings();
    }

    void Update()
    {
        bool interactionDetected = false;

        // Check for mouse clicks.
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            interactionDetected = true;
        }

        // Check if the main camera has moved more than the threshold.
        if (Camera.main != null)
        {
            float moveDistance = Vector3.Distance(Camera.main.transform.position, lastCameraPosition);
            if (moveDistance > cameraMovementThreshold)
            {
                interactionDetected = true;
            }
            lastCameraPosition = Camera.main.transform.position;
        }

        if (interactionDetected)
        {
            // Reset timer and trigger fade out if object is active.
            timer = 0f;
            if (targetObject.activeSelf)
            {
                TriggerFadeOut();
            }
        }
        else
        {
            timer += Time.deltaTime;
            // If idle time is reached and the object is not fully visible, trigger fade in.
            if (timer >= idleTime && (!targetObject.activeSelf || GetCurrentAlpha() < targetAlpha - 0.01f))
            {
                if (!targetObject.activeSelf)
                {
                    targetObject.SetActive(true);
                }
                TriggerFadeIn();
            }
        }
    }

    /// <summary>
    /// Returns the current alpha from the first SpriteRenderer found (assumes all share the same alpha).
    /// </summary>
    float GetCurrentAlpha()
    {
        SpriteRenderer sr = targetObject.GetComponentInChildren<SpriteRenderer>();
        return (sr != null) ? sr.color.a : 0f;
    }

    /// <summary>
    /// Sets the alpha for all SpriteRenderers and VideoPlayers (with CameraFarPlane) in the target object and its children.
    /// </summary>
    void SetChildrenAlpha(float alpha)
    {
        // Update SpriteRenderers.
        SpriteRenderer[] sprites = targetObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in sprites)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        // Update VideoPlayers with CameraFarPlane render mode.
        VideoPlayer[] videoPlayers = targetObject.GetComponentsInChildren<VideoPlayer>();
        foreach (VideoPlayer vp in videoPlayers)
        {
            if (vp.renderMode == VideoRenderMode.CameraFarPlane || vp.renderMode == VideoRenderMode.CameraNearPlane)
            {
                vp.targetCameraAlpha = alpha;
            }
        }
    }

    /// <summary>
    /// Disables all Animator components in the target object and its children.
    /// </summary>
    void StopChildAnimations()
    {
        Animator[] animators = targetObject.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            animator.enabled = false;
        }
    }

    /// <summary>
    /// Enables all Animator components in the target object and its children.
    /// </summary>
    void StartChildAnimations()
    {
        Animator[] animators = targetObject.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            animator.enabled = true;
        }
    }

    /// <summary>
    /// Starts a fade-in: re-enables child animators and fades children to targetAlpha.
    /// </summary>
    void TriggerFadeIn()
    {
        // Re-enable any animators so they can resume once fade in is complete.
        StartChildAnimations();
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeChildrenToAlpha(targetAlpha, fadeInDuration, null));
    }

    /// <summary>
    /// Starts a fade-out: stops child animators and fades children to 0, then deactivates the object.
    /// </summary>
    void TriggerFadeOut()
    {
        StopChildAnimations();
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeChildrenToAlpha(0f, fadeOutDuration, () => targetObject.SetActive(false)));
    }

    /// <summary>
    /// Coroutine that fades all child SpriteRenderers and VideoPlayers (with CameraFarPlane) to the specified alpha over duration.
    /// Calls onComplete when finished.
    /// </summary>
    IEnumerator FadeChildrenToAlpha(float endAlpha, float duration, System.Action onComplete)
    {
        // Get SpriteRenderers and record starting alphas.
        SpriteRenderer[] sprites = targetObject.GetComponentsInChildren<SpriteRenderer>();
        float[] startAlphas = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            startAlphas[i] = sprites[i].color.a;
        }

        // Get VideoPlayers with CameraFarPlane render mode and record starting alphas.
        VideoPlayer[] videoPlayers = targetObject.GetComponentsInChildren<VideoPlayer>();
        System.Collections.Generic.List<VideoPlayer> validVideoPlayers = new System.Collections.Generic.List<VideoPlayer>();
        System.Collections.Generic.List<float> startVideoAlphas = new System.Collections.Generic.List<float>();
        foreach (VideoPlayer vp in videoPlayers)
        {
            if (vp.renderMode == VideoRenderMode.CameraFarPlane || vp.renderMode == VideoRenderMode.CameraNearPlane)
            {
                validVideoPlayers.Add(vp);
                startVideoAlphas.Add(vp.targetCameraAlpha);
            }
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Lerp SpriteRenderer alphas.
            for (int i = 0; i < sprites.Length; i++)
            {
                Color c = sprites[i].color;
                c.a = Mathf.Lerp(startAlphas[i], endAlpha, t);
                sprites[i].color = c;
            }

            // Lerp VideoPlayer targetCameraAlpha for those using CameraFarPlane.
            for (int i = 0; i < validVideoPlayers.Count; i++)
            {
                validVideoPlayers[i].targetCameraAlpha = Mathf.Lerp(startVideoAlphas[i], endAlpha, t);
            }
            yield return null;
        }
        // Ensure final alpha is set.
        SetChildrenAlpha(endAlpha);
        fadeCoroutine = null;
        if (onComplete != null)
            onComplete();
    }

    // --- Saving and Loading Player Prefs ---
    void SaveSettings()
    {
        PlayerPrefs.SetFloat(KEY_IDLE_TIME, idleTime);
        PlayerPrefs.SetFloat(KEY_FADE_IN, fadeInDuration);
        PlayerPrefs.SetFloat(KEY_FADE_OUT, fadeOutDuration);
        PlayerPrefs.Save();
        Debug.Log("Settings saved.");
    }

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey(KEY_IDLE_TIME))
        {
            idleTime = PlayerPrefs.GetFloat(KEY_IDLE_TIME);
        }
        if (PlayerPrefs.HasKey(KEY_FADE_IN))
        {
            fadeInDuration = PlayerPrefs.GetFloat(KEY_FADE_IN);
        }
        if (PlayerPrefs.HasKey(KEY_FADE_OUT))
        {
            fadeOutDuration = PlayerPrefs.GetFloat(KEY_FADE_OUT);
        }
        Debug.Log("Settings loaded.");
    }

    void OnApplicationQuit()
    {
        SaveSettings();
    }
}
