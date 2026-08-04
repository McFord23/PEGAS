using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider player1SensitivitySlider;
    public Slider player2SensitivitySlider;

    private void Start()
    {
        print($"sens: {Global.players[0].sensitivity}");
        player1SensitivitySlider.value = Global.players[0].sensitivity;
        player2SensitivitySlider.value = Global.players[1].sensitivity;
    }

    public void SetPlayer1Sensitivity(float value)
    {
        Global.players[0].sensitivity = value;
    }

    public void SetPlayer2Sensitivity(float value)
    {
        Global.players[1].sensitivity = value;
    }
}
