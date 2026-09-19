using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Vignette : MonoBehaviour
{
    private const float DISABLED_ALPHA = 0.25f;
    private const float ENABLED_ALPHA = 1f;
    private const float SPEED = 0.01f;

    [SerializeField] private Image image;
    
    private Coroutine coroutine;

    public void SetActive(bool value)
    {
        var alpha = value ? ENABLED_ALPHA : DISABLED_ALPHA;
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(SetAlpha(alpha));
    }

    private IEnumerator SetAlpha(float value)
    {
        var startColor = image.color;
        var targetColor = new Color(image.color.r, image.color.g, image.color.b, value);
        float progress = 0;

        while (progress < 1)
        {
            image.color = Color.Lerp(startColor, targetColor, progress);
            progress += SPEED;
            yield return null;
        }

        coroutine = null;
    }
}