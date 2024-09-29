using UnityEngine;

public class UpdateDrawLayer : MonoBehaviour
{
    [SerializeField] int layerWhenInfront;
    [SerializeField] int layerWhenBehind;
    [SerializeField] Renderer GFX;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GFX.sortingOrder = layerWhenBehind;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GFX.sortingOrder = layerWhenInfront;
    }
}
