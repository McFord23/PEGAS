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
        if (LevelManager.IsMenuLevel())
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
        LevelManager.Instance.LoadScene(Level.MainMenu);
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
