using System.Collections;
using UnityEngine;
using TMPro;

public class Info : MonoBehaviour
{
    private const float MOVE_SPEED = 4f;
    private const float COLOR_SPEED = 2;
    
    [SerializeField] private Vector3 startPos = new Vector3(0, 0.74f, 0);
    
    private TextMeshPro label;
    private Color showColor;
    private Color hideColor;

    private Coroutine showing;

    private void Start()
    {
        label = GetComponent<TextMeshPro>();
        showColor = label.color;
        hideColor = showColor;
        hideColor.a = 0;
        SetColor(hideColor);
        
        transform.localPosition = startPos;
    }

    public void Show(string info, Color color)
    {
        if (showing != null) StopCoroutine(showing);
        StartCoroutine(ShowCoroutine(info, color));
    }
    
    private IEnumerator ShowCoroutine(string info, Color color)
    {
        label.text = info;
        label.color = color;
        
        transform.localPosition = startPos;
        var targetPos = new Vector3(0, startPos.y + 1.25f, 0);

        StartCoroutine(Utilities.ColorLerp(SetColor, GetColor, showColor, COLOR_SPEED));
        yield return StartCoroutine(Utilities.LocalPosSlerp(transform, targetPos, MOVE_SPEED));

        targetPos = new Vector3(0, startPos.y + 1f, 0);
        yield return StartCoroutine(Utilities.LocalPosSlerp(transform, targetPos, MOVE_SPEED / 2));
        
        targetPos = new Vector3(0, startPos.y + 1.5f, 0);
        StartCoroutine(Utilities.LocalPosSlerp(transform, targetPos, MOVE_SPEED));
        yield return StartCoroutine(Utilities.ColorLerp(SetColor, GetColor, hideColor, COLOR_SPEED));

        showing = null;
    }

    private void SetColor(Color color)
    {
        label.color = color;
    }

    private Color GetColor()
    {
        return label.color;
    }
}
