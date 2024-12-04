using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void Use()
    {
        ItemManager.UseHeldItem(gameObject);
    }
}
