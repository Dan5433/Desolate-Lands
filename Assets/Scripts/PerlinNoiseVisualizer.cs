using EditorAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PerlinNoiseVisualizer : MonoBehaviour
{
    [SerializeField] int pixWidth = 32;
    [SerializeField] int pixHeight = 32;
    [SerializeField] float xOrg;
    [SerializeField] float yOrg;
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
                float xCoord = xOrg + (float)x / pixWidth * scale;
                float yCoord = yOrg + (float)y / pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[y * pixWidth + x] = new Color(sample, sample, sample);
            }
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }
}
