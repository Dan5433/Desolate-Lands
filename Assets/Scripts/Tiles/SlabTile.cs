using UnityEngine;

public class SlabTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.parent.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.transform.parent.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
    }
}
