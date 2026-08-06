using UnityEngine;

public class DeadMenu : MonoBehaviour
{
    public void Retry()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.DisableMenu();
    }

    public void Exit()
    {
        SceneManagerAdapter.Instance.LoadScene("Main Menu");
    }
}
