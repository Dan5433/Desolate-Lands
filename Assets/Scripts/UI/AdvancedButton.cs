using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AdvancedButton : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler 
    //IPointerEnterHandler
{
    [SerializeField] Color normal;
    [SerializeField] Color hover;
    [SerializeField] Color held;
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
