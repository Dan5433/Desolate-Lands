using UnityEngine;
using UnityEngine.UI;

public class MinimapPointerSprites : MonoBehaviour
{
    [SerializeField] Sprite up;
    [SerializeField] Sprite down;
    [SerializeField] Sprite left;
    [SerializeField] Sprite right;
    [SerializeField] Image image;

    public void UpdateSprite(Direction direction)
    {
        image.sprite = direction switch
        {
            Direction.Up => up,
            Direction.Right => right,
            Direction.Down => down,
            Direction.Left => left,
            _ => up
        };
    }
}
