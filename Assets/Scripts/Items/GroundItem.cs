using CustomClasses;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : MonoBehaviour
{
    [SerializeField] InvItem item;
    [SerializeField] float groupDistance = 0.8f;
    Image image;

    public InvItem InvItem { get { return item; } set { item = value; } }

    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = item.ItemObj.Sprite;

        Invoke(nameof(GroupItems), 0.1f);
        if(item is InvTool tool)
        {
            Debug.Log(tool.Durability);
        }
    }

    void GroupItems()
    {
        int mask = LayerMask.GetMask("Item");

        var nearbyItems = Physics2D.OverlapCircleAll(transform.position, groupDistance, mask);

        foreach (Collider2D collider in nearbyItems)
        {
            var otherItem = collider.GetComponent<GroundItem>().InvItem;

            if (item.Count == item.ItemObj.MaxCount) { return; }
            if (collider.gameObject == gameObject || otherItem.Count == otherItem.ItemObj.MaxCount) { continue; }

            if (otherItem.ItemObj == item.ItemObj && otherItem.Name == item.Name)
            {
                if (otherItem.Count + item.Count <= item.ItemObj.MaxCount)
                {
                    item.Count += otherItem.Count;
                    Destroy(collider.gameObject);
                }
                else
                {
                    otherItem.Count -= item.ItemObj.MaxCount - item.Count;
                    item.Count = item.ItemObj.MaxCount;
                }
            }
        }
    }
}
