using UnityEngine;
using UnityEngine.UI;

public class BookMark : MonoBehaviour
{
    [SerializeField] private float flippedPositionX;
    private float positionX;
    
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform icon;
    private RectTransform rect;
    private Toggle toggle;
    
    public void Initialize()
    {
        positionX = icon.anchoredPosition.x;
        rect = transform as RectTransform;
        toggle = GetComponent<Toggle>();
    }

    public void SetActive(bool value)
    {
        toggle.isOn = value;
    }
    
    public void Flip(bool isFlipToRight)
    {
        if (isFlipToRight)
        {
            if (background.localScale.x > 0) return;
            icon.anchoredPosition = new Vector2(positionX, icon.anchoredPosition.y);
        }
        else
        {
            if (background.localScale.x < 0) return;
            icon.anchoredPosition = new Vector2(flippedPositionX, icon.anchoredPosition.y);
        }

        var pos = rect.localPosition;
        pos.x = -pos.x;
        rect.localPosition = pos;
        
        var scale = background.localScale;
        scale.x = -scale.x;
        background.localScale = scale;
    }
}