using UnityEngine;

public class SlabTile : MonoBehaviour
{
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    collision.transform.parent.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        var yRelation = collision.transform.parent.position.y - transform.position.y - 0.4f;
        print(yRelation);
        if (yRelation < -0.1f && yRelation > -0.39f)
        {
            collision.transform.parent.position =
                new(collision.transform.parent.position.x,
                collision.transform.parent.position.y + 0.25f,
                collision.transform.parent.position.z);
        }
        if (yRelation > -0.1f && yRelation < 0)
        {
            collision.transform.parent.position =
                new(collision.transform.parent.position.x,
                collision.transform.parent.position.y - 0.25f,
                collision.transform.parent.position.z);
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    collision.transform.parent.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
    //}
}
