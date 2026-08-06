using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider player1SensitivitySlider;
    [SerializeField] private Slider player2SensitivitySlider;

    private void Start()
    {
        player1SensitivitySlider.value = PlayersSettings.Player1.Sensitivity;
        player2SensitivitySlider.value = PlayersSettings.Player2.Sensitivity;
    }

    public void SetPlayer1Sensitivity(float value)
    {
        PlayersSettings.Player1.Sensitivity = value;
    }

    public void SetPlayer2Sensitivity(float value)
    {
        PlayersSettings.Player2.Sensitivity = value;
    }
}
