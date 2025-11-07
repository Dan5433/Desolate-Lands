using EditorAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PerlinNoiseVisualizer : MonoBehaviour
{
    [Title("Uses transform position for noise origin")]
    [SerializeField] Color minColor = Color.black;
    [SerializeField] Color maxColor = Color.white;
    [SerializeField] int pixWidth = 32;
    [SerializeField] int pixHeight = 32;
    [SerializeField] float scale = 1f;

    [Button(buttonHeight: 36)]
    void GenerateNoise()
    {
        Texture2D noiseTex = new(pixWidth, pixHeight);
        Color[] pix = new Color[pixWidth * pixHeight];
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(noiseTex, new Rect(0, 0, pixWidth, pixHeight), Vector2.zero, 1);

        for (int y = 0; y < pixHeight; y++)
        {
            for (int x = 0; x < pixWidth; x++)
            {
                float xCoord = transform.position.x + (float)x / pixWidth * scale;
                float yCoord = transform.position.y + (float)y / pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[y * pixWidth + x] = Color.Lerp(minColor, maxColor, sample);
            }
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        GetComponent<RectTransform>().sizeDelta = new(pixWidth, pixHeight);
    }
}
