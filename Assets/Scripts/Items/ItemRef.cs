using CustomClasses;
using UnityEngine;

public class ItemRef : MonoBehaviour
{
    [SerializeField] InvItem item;

    public InvItem Item { get { return item; } set { item = value; } }
}
