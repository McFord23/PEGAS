using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ExecuteResume();
        }
    }

    public void ExecuteResume()
    {
        EventAdapter.Instance.Execute(EventKey.Resume);
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.DisableMenu();
        PlayersManager.Instance.Resume();
    }

    public void Exit()
    {
        Global.players[0].live = true;
        Global.players[1].live = true;
        SceneManagerAdapter.Instance.LoadScene("Main Menu");
    }
}
