using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AdvancedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] UnityEvent onMouseDown;
    [SerializeField] UnityEvent onMouseUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        onMouseDown?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseUp?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onMouseUp?.Invoke();
    }

    void OnDisable()
    {
        onMouseUp?.Invoke();
    }
}
