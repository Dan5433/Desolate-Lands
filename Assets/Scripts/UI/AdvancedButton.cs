using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AdvancedButton : Button
{
    [SerializeField] UnityEvent onMouseDown;
    [SerializeField] UnityEvent onMouseUp;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (!IsActive() || !IsInteractable())
            return;

        onMouseDown?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onMouseUp?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onMouseUp?.Invoke();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onMouseUp?.Invoke();
    }
}
