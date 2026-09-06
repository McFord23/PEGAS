using UnityEngine;
using TMPro;

public class AdressField : MonoBehaviour
{
    private TMP_InputField inputField;
    private TMP_Text text;

    private AddressFieldManager manager;
    private NetworkSubmenu networkSubmenu;

    private const float COOLDOWN = 1f;
    private float timer = 1f;

    public void Initialize()
    {
        inputField = GetComponent<TMP_InputField>();
        text = inputField.transform.GetChild(0).Find("Text").GetComponent<TMP_Text>();

        manager = transform.GetComponentInParent<AddressFieldManager>();
        networkSubmenu = manager.GetComponentInParent<NetworkSubmenu>();
    }

    private void Update()
    {
        if (!inputField.isFocused) return;
        
        if (Controls.Paste)
        {
            if (timer >= COOLDOWN)
            {
                manager.Paste();
                timer = 0;
            }
        }

        if (timer < COOLDOWN) timer += Time.deltaTime;
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
            text.color = Settings.Red;
            networkSubmenu.InvalidIP();
        }
        else
        {
            text.color = Settings.Brown;
            manager.ChangeIP();
            manager.Save();
            networkSubmenu.ValidIP();
        }
    }
}
