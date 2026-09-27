using UnityEngine;

public class MainMenu : MenuBase
{
    [SerializeField] private GameObject lockLeft;
    [SerializeField] private GameObject lockRight;
    [SerializeField] private GameObject returnPopup;

    public override void UpdateMark(bool value)
    {
        lockLeft.SetActive(!gameObject.activeSelf);
        lockRight.SetActive(!gameObject.activeSelf);
    }

    public void Open()
    {
        if (LevelsManager.IsMenuLevel())
        {
            SetActive(true);
        }
        else
        {
            Manager.ShowPopup(returnPopup);
        }
    }

    public void Load()
    {
        LevelsManager.Instance.LoadLevel(Level.MainMenu);
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
