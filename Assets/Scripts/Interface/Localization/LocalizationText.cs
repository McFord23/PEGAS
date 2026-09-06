using TMPro;

public class LocalizationText : LocalizationBase
{
    private TextMeshPro textPro;
    private TextMeshProUGUI textProGUI;
    private Mode mode;

    private enum Mode
    {
        TextMeshPro,
        TextMeshProUGUI
    }
    
    protected override void Start()
    {
        if (TryGetComponent(out TextMeshPro tmp))
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
        
        switch (mode)
        {
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
        return LocalizationManager.GetPhrase(file, phrase);
    }
}
