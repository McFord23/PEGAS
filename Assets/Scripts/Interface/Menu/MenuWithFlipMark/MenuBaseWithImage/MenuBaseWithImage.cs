using UnityEngine;

public class MenuBaseWithImage : MenuBaseWithFlipMark
{
    [SerializeField] private bool isImageOnLeft;
    [SerializeField] protected GameObject image;
    private RectTransform imageRect;
    private MenuBackground.ImagePageType currentImagePage;
    private Mode mode;
    
    private enum Mode
    {
        Image,
        Hole
    }

    public override void Initialize(MenuManager manager, MenuBackground background)
    {
        base.Initialize(manager, background);

        imageRect = image.GetComponent<RectTransform>();

        if (SceneManagerAdapter.IsMenuScene())
        {
            ChangeModeToImage();
            ChangePage(MenuBackground.ImagePageType.Empty);
        }
        else
        {
            ChangeModeToHole();
            ChangePage(MenuBackground.ImagePageType.Arch);
        }
    }
    
    public override void SetActive(bool value)
    {
        base.SetActive(value);

        switch (mode)
        {
            case Mode.Image:
                image.SetActive(value);
                FlipImage(isImageOnLeft);
                break;
            
            case Mode.Hole:
                Background.SetMask(value);
                break;
        }
    }

    private void ChangeModeToImage()
    {
        Background.SetMask(false);
        image.SetActive(true);
        mode = Mode.Image;
    }

    private void ChangeModeToHole()
    {
        image.SetActive(false);
        Background.SetMask(true);
        mode = Mode.Hole;
    }

    protected void ChangePage(MenuBackground.ImagePageType imagePageType)
    {
        Page.sprite = Background.GetImagePage(imagePageType, isImageOnLeft);
    }

    protected void FlipImage(bool isToLeft)
    {
        if ((isToLeft && (imageRect.anchoredPosition.x > 0)) || (!isToLeft && (imageRect.anchoredPosition.x < 0)))
        {
            imageRect.anchoredPosition = new Vector2(-imageRect.anchoredPosition.x, imageRect.anchoredPosition.y);
        }
        
        isImageOnLeft = isToLeft;
        ChangePage(currentImagePage);
    }
}