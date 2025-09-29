using UnityEngine;
using UnityEngine.UI;

public class SimpleScratch : MonoBehaviour
{
    public RawImage dirtImage;   // RawImage z brudem
    public int eraseRadius = 40; // promień ścierania w pikselach

    private Texture2D dirtTexture;
    private RectTransform dirtRect;

    void Start()
    {
        dirtRect = dirtImage.rectTransform;

        // kopiujemy teksturę brudu, żeby można było ją modyfikować
        Texture2D sourceTex = dirtImage.texture as Texture2D;
        dirtTexture = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.RGBA32, false);
        dirtTexture.SetPixels(sourceTex.GetPixels());
        dirtTexture.Apply();

        dirtImage.texture = dirtTexture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // trzymanie myszy/palca
        {
            Debug.Log("Klik działa!");
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dirtRect, Input.mousePosition, null, out localPos))
            {
                // lokalna pozycja (środek = 0,0) -> normalizacja
                float normalizedX = (localPos.x / dirtRect.rect.width) + 0.5f;
                float normalizedY = (localPos.y / dirtRect.rect.height) + 0.5f;

                int x = Mathf.RoundToInt(normalizedX * dirtTexture.width);
                int y = Mathf.RoundToInt(normalizedY * dirtTexture.height);

                EraseCircle(x, y);
            }

        }
    }

    void EraseCircle(int centerX, int centerY)
    {
        for (int i = -eraseRadius; i <= eraseRadius; i++)
        {
            for (int j = -eraseRadius; j <= eraseRadius; j++)
            {
                int px = centerX + i;
                int py = centerY + j;

                if (px >= 0 && px < dirtTexture.width && py >= 0 && py < dirtTexture.height)
                {
                    if (i * i + j * j <= eraseRadius * eraseRadius) // okrąg
                    {
                        Color c = dirtTexture.GetPixel(px, py);
                        c.a = 0; // wymazanie → przezroczystość
                        dirtTexture.SetPixel(px, py, c);
                    }
                }
            }
        }
        dirtTexture.Apply();
    }
}