using UnityEngine.UI;
using TMPro;

public class LocalizationText : LocalizationBase
{
    private Text text;
    private TextMeshPro textPro;
    private TextMeshProUGUI textProGUI;
    private Mode mode;

    private enum Mode
    {
        Text,
        TextMeshPro,
        TextMeshProUGUI
    }
    
    protected override void Start()
    {
        if (TryGetComponent(out Text txt))
        {
            text = txt;
            mode = Mode.Text;
        }
        else if (TryGetComponent(out TextMeshPro tmp))
        {
            textPro = tmp;
            mode = Mode.TextMeshPro;
        }
        else if (TryGetComponent(out TextMeshProUGUI tmpGUI))
        {
            textProGUI = tmpGUI;
            mode = Mode.TextMeshProUGUI;
        }

        base.Start();
    }

    protected override void Localize()
    { 
        base.Localize();
        
        if (IsPathEmpty) return;
        //print($"localize {name}");
        
        switch (mode)
        {
            case Mode.Text:
                text.text = GetPhrase();
                break;
            
            case Mode.TextMeshPro:
                textPro.text = GetPhrase();
                break;
            
            case Mode.TextMeshProUGUI:
                textProGUI.text = GetPhrase();
                break;
        }
    }

    protected virtual string GetPhrase()
    {
        return LocalizationManager.Instance.GetPhrase(file, phrase);
    }
}
