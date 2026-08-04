using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineLayout : MonoBehaviour
{
    Button button;
    Button layout;
    Button nextLayout;
    Button perviousLayout;

    bool isSetSelected = false;

    void Start()
    {
        button = GetComponent<Button>();
        layout = transform.Find("Outline").GetComponent<Button>();
        nextLayout = transform.Find("Next Layout").GetComponent<Button>();
        perviousLayout = transform.Find("Pervious Layout").GetComponent<Button>();
    }

    void Update()
    {
        if (isSetSelected)
        {
            if (Controls.Pause)
            {
                button.Select();
                isSetSelected = false;
            }

            if (Controls.Move.x > 0)
            {
                nextLayout.onClick.Invoke();
            }
            else if (Controls.Move.x < 0)
            {
                perviousLayout.onClick.Invoke();
            }
        }
    }

    public void Press()
    {
        layout.Select();
        isSetSelected = true;
    }
}
