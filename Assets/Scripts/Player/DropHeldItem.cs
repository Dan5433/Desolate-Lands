using UnityEngine;
using UnityEngine.EventSystems;

public class DropHeldItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,IPointerMoveHandler
{
    [SerializeField] Transform origin;
    [SerializeField] float throwDistance;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != 0) 
            return;

        var hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero);

        if (GameManager.CursorState == CursorState.Use)
        {
            hit.transform.GetComponent<UseItem>().Use();
            return;
        }

        if(ItemManager.DropHeldItem(origin.position, origin.up, throwDistance))
            origin.GetComponentInParent<Interact>().DisableUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ItemManager.Instance.IsHoldingItem) 
            return;

        GameManager.CursorState = CursorState.Drop;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.CursorState = CursorState.Default;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        var hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero);

        if (hit && hit.transform.GetComponent<UseItem>() && ItemManager.IsHeldItemUsable())
        {
            GameManager.CursorState = CursorState.Use;
        }
        else if (ItemManager.Instance.IsHoldingItem)
        {
            GameManager.CursorState = CursorState.Drop;
        }
    }
}
