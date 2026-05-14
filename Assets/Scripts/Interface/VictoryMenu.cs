using UnityEngine;

public class VictoryMenu : MonoBehaviour
{
    public void ToBeContinued()
    {
        SceneManagerAdapter.Instance.LoadScene("Credits");
    }
}
