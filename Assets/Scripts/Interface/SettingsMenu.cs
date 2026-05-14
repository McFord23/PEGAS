using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private Slider mouseSlider;
    private Slider keyboardSlider;

    private void Start()
    {
        mouseSlider = transform.Find("Mouse Slider").gameObject.GetComponent<Slider>();
        keyboardSlider = transform.Find("Keyboard Slider").gameObject.GetComponent<Slider>();

        mouseSlider.value = Global.Sensitivity.mouse;
        keyboardSlider.value = Global.Sensitivity.keyboard;
    }

    public void SetMouseSensitivity(float value)
    {
        Global.Sensitivity.mouse = value;
    }

    public void SetKeyboardSensitivity(float value)
    {
        Global.Sensitivity.keyboard = value;
    }
}
