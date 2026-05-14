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
        Global.players[0].live = true;
        Global.players[1].live = true;
        SceneManagerAdapter.Instance.LoadScene("Main Menu");
    }
}
