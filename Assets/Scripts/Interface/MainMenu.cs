using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManagerAdapter.Instance.LoadScene("Game");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
