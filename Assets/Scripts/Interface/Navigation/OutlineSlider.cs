using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineSlider : MonoBehaviour
{
    public MenuManager menu;
    Button button;
    Slider slider;
    bool isSliderSelected = false;

    void Start()
    {
        button = GetComponent<Button>();
        slider = GetComponentInParent<Slider>();
    }

    void Update()
    {
        if (isSliderSelected)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                button.Select();
                menu.SelectedState(false);
            }
        }
    }

    public void Press()
    {
        slider.Select();
        isSliderSelected = true;
        menu.SelectedState(true);
    }
}
