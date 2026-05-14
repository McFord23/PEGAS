using UnityEngine;
using TMPro;

public class AdressField : MonoBehaviour
{
    private TMP_InputField inputField;
    private TMP_Text text;
    private Color brown;
    private Color red;

    private AddressFieldManager manager;
    private NetworkSubmenu networkSubmenu;

    private float cooldown = 1f;
    private float timer = 1f;

    public void Initialize()
    {
        inputField = GetComponent<TMP_InputField>();
        text = inputField.transform.GetChild(0).Find("Text").GetComponent<TMP_Text>();
        brown = text.color;
        red = red = new Color(0.45f, 0.2f, 0.15f);

        manager = transform.GetComponentInParent<AddressFieldManager>();
        networkSubmenu = manager.GetComponentInParent<NetworkSubmenu>();
    }

    private void Update()
    {
        if (!inputField.isFocused) return;
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.V))
        {
            if (timer >= cooldown)
            {
                manager.Paste();
                timer = 0;
            }
        }

        if (timer < cooldown) timer += Time.deltaTime;
    }

    public void OnInput()
    {
        if (inputField.caretPosition == inputField.characterLimit)
        {
            manager.Next(inputField);
        }
    }

    public void OnEndEdit()
    {
        if (name == "Port") return;
        
        int ipPart = int.Parse(inputField.text);

        if (ipPart > 255)
        {
            text.color = red;

            string error = "invalid ip";
            networkSubmenu.ShowError(error, true);
        }
        else
        {
            text.color = brown;
            manager.ChangeIP();
            manager.Save();
            networkSubmenu.HideStatus();
        }
    }
}
