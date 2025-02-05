using CustomExtensions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorPulse : MonoBehaviour
{
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;
    [SerializeField] float speed;

    private void OnEnable()
    {
        StartCoroutine(PulseColor());
    }

    private void OnDisable()
    {
        StopCoroutine(PulseColor());
    }

    public void ResetPulse()
    {
        StopAllCoroutines();
        StartCoroutine(PulseColor());
    }

    IEnumerator PulseColor()
    {
        var image = GetComponent<Image>();
        image.color = startColor;

        float time = 0;
        while (true)
        {
            while (image.color != endColor)
            {
                time += Time.deltaTime * speed;
                image.color = Color.Lerp(startColor, endColor, time);
                yield return null;
            };
            time = 0;

            while (image.color != startColor)
            {
                time += Time.deltaTime * speed;
                image.color = Color.Lerp(endColor, startColor, time);
                yield return null;
            };
            time = 0;
        }
    }
}
