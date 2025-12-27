using EditorAttributes;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PerlinNoiseVisualizer : MonoBehaviour
{
    [SerializeField] Color minColor = Color.black;
    [SerializeField] Color maxColor = Color.white;
    [SerializeField] Vector2 position;
    [SerializeField][Tooltip("Width x Height")] Vector2Int size;
    [SerializeField] float scale = 1f;

    [Button(buttonHeight: 36)]
    void GenerateNoise()
    {
        Texture2D noiseTex = new(size.x, size.y);
        Color[] pix = new Color[size.x * size.y];
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(noiseTex, new Rect(0, 0, size.x, size.y), Vector2.zero, 1);

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector2 coordinates = position + new Vector2(x, y) / size * scale;

                float sample = noise.cnoise(coordinates);
                pix[y * size.x + x] = Color.Lerp(minColor, maxColor, sample);
            }
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        GetComponent<RectTransform>().sizeDelta = new(size.x, size.y);
    }
}
