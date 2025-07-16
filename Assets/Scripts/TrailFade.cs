using UnityEngine;

public class TrailFade : MonoBehaviour
{
    // Total time before the trail is destroyed.
    public float lifetime = 1f;

    // The duration over which the sprite fades out.
    public float fadeDuration = 1f;

    private float timer = 0f;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (spriteRenderer != null)
        {
            // Fade out over the lifetime.
            float alpha = Mathf.Lerp(initialColor.a, 0f, timer / lifetime);
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
