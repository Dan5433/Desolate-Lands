using UnityEngine;

public class MinimapPointerSprites : MonoBehaviour
{
    [SerializeField] Sprite up;
    [SerializeField] Sprite down;
    [SerializeField] Sprite left;
    [SerializeField] Sprite right;
    [SerializeField] SpriteRenderer spriteRender;

    public void UpdateSprite(Direction direction)
    {
        spriteRender.sprite = direction switch
        {
            Direction.Up => up,
            Direction.Right => right,
            Direction.Down => down,
            Direction.Left => left,
            _ => up
        };
    }
}
