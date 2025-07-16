using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowAnimator : MonoBehaviour
{
    public float glowSpeed = 2f; // How fast the glow pulses.
    public float maxGlow = 5f;
    public float minGlow = 0f;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        // Simple pulsating glow effect.
        float glow = Mathf.PingPong(Time.time * glowSpeed, maxGlow - minGlow) + minGlow;
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_GlowIntensity", glow);
        spriteRenderer.SetPropertyBlock(propBlock);
    }
}
