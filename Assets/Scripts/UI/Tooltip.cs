using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] Vector3 mouseOffset;
    [SerializeField] TMP_Text main;
    [SerializeField] TMP_Text extra;
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        main.text = message;
        gameObject.SetActive(true);
    }

    public void ShowMessage(string message, string extra)
    {
        main.text = message;
        this.extra.text = extra;
        gameObject.SetActive(true);
    }

    public void UpdatePosition()
    {
        Vector2 position = Input.mousePosition + mouseOffset;

        var tooltipTransform = GetComponent<RectTransform>();

        tooltipTransform.position = position;

        //Use anchored position for canvas related calculations
        Vector2 anchoredPosition = tooltipTransform.anchoredPosition;
        var canvasSize = tooltipTransform.parent.GetComponent<RectTransform>().sizeDelta;

        //Force inside of canvas
        if (tooltipTransform.anchoredPosition.x + tooltipTransform.sizeDelta.x > canvasSize.x)
        {
            anchoredPosition.x -= tooltipTransform.sizeDelta.x + mouseOffset.x * 4;
        }

        if (tooltipTransform.anchoredPosition.y + tooltipTransform.sizeDelta.y > canvasSize.y)
        {
            anchoredPosition.y -= tooltipTransform.sizeDelta.y + mouseOffset.y * 4;
        }

        tooltipTransform.anchoredPosition = anchoredPosition;
    }
}
