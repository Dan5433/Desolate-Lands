using UnityEngine;
using UnityEngine.EventSystems;

public class DropHeldItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Transform player;
    [SerializeField] float throwDistance;
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == 0)
        {
            ItemManager.DropHeldItem(player.position, player.up, throwDistance);
        }
    }
}
