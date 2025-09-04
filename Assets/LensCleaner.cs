using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LensCleaner : MonoBehaviour
{
    [Header("UI Elements")]
    public RawImage dirtImage;
    public Camera uiCamera;
    public TMP_Text messageText; 

    [Header("Brush Settings")]
    public int brushSize = 20;
    [Range(0, 1)]
    public float completionThreshold = 0.8f; 

    private Texture2D dirtTex;
    private RectTransform rt;
    private bool completed = false;

    void Start()
    {
        if (uiCamera == null)
            uiCamera = dirtImage.canvas.worldCamera;

        rt = dirtImage.rectTransform;

        
        dirtTex = Instantiate(dirtImage.texture as Texture2D);
        dirtImage.texture = dirtTex;

        if (messageText != null)
            messageText.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !completed)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, uiCamera, out localPoint))
            {
                float px = (localPoint.x + rt.rect.width * 0.5f) / rt.rect.width * dirtTex.width;
                float py = (localPoint.y + rt.rect.height * 0.5f) / rt.rect.height * dirtTex.height;

                ClearCircle((int)px, (int)py);
                CheckCompletion();
            }
        }
    }

    void ClearCircle(int cx, int cy)
    {
        int r = brushSize;

        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px >= 0 && px < dirtTex.width && py >= 0 && py < dirtTex.height)
                    {
                        Color c = dirtTex.GetPixel(px, py);
                        c.a = 0f;
                        dirtTex.SetPixel(px, py, c);
                    }
                }
            }
        }
        dirtTex.Apply();
    }

    void CheckCompletion()
    {
        int transparent = 0;
        int total = dirtTex.width * dirtTex.height;

        Color[] pixels = dirtTex.GetPixels();
        foreach (Color c in pixels)
        {
            if (c.a == 0f)
                transparent++;
        }

        float percent = (float)transparent / total;

        if (percent >= completionThreshold)
        {
            completed = true;
            if (messageText != null)
            {
                messageText.gameObject.SetActive(true);
                messageText.text = "You did it!";
            }

            
            dirtImage.gameObject.SetActive(false);
        }
    }

    public bool IsCompleted()
    {
        return completed;
    }
}
