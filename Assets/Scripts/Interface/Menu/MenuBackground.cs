using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour
{
    [FormerlySerializedAs("imagePage")]
    [SerializeField] private Sprite emptyLeftPage;
    [SerializeField] private Sprite emptyRightPage;
    [SerializeField] private Sprite archLeftPage;
    [SerializeField] private Sprite archRightPage;
    [SerializeField] private Sprite infoRightPage;
    
    [Header("Mask")]
    [SerializeField] private Image backgroundMask;
    
    [Header("Moving")]
    [SerializeField] private Vector2 monoPagePosition = new (-310, 0);
    [SerializeField] private Vector2 dualPagePosition = Vector2.zero;
    [SerializeField] private float speed = 0.1f;
    private RectTransform menuTransform;
    private IEnumerator move;

    public enum ImagePageType
    {
        Empty,
        Arch,
        Info
    }
    
    public void Initialize()
    {
        menuTransform = GetComponent<RectTransform>();
    }
    
    public Sprite GetImagePage(ImagePageType imagePageType, bool isImageOnLeft)
    {
        return imagePageType switch
        {
            ImagePageType.Empty => isImageOnLeft ? emptyLeftPage : emptyRightPage,
            ImagePageType.Arch => isImageOnLeft ? archLeftPage : archRightPage,
            ImagePageType.Info => infoRightPage,
            _ => throw new ArgumentOutOfRangeException(nameof(imagePageType), imagePageType, null)
        };
    }
    
    public void SetMask(bool isOn)
    {
        var color = backgroundMask.color;
        color.a = isOn ? 1 : 0;
        backgroundMask.color = color;
    }
    
    public void OnChangeMenu(MenuBase menu)
    {
        var target = menu.IsMonoPage() ? monoPagePosition : dualPagePosition;
        TryMove(target);
    }

    private void TryMove(Vector2 target)
    {
        if (menuTransform.anchoredPosition == target) return;
        if (move != null) StopCoroutine(move);

        move = Move(target);
        StartCoroutine(move);
    }

    private IEnumerator Move(Vector2 target)
    {
        var startPos = menuTransform.anchoredPosition;
        float progress = 0;
        
        while (menuTransform.anchoredPosition != target)
        {
            menuTransform.anchoredPosition = Utilities.Vector2Slerp(startPos, target, progress);
            progress += speed;
            
            yield return new WaitForFixedUpdate();
        }

        move = null;
    }
}