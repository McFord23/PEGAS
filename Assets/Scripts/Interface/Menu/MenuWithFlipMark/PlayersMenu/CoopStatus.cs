using System.Collections;
using UnityEngine;
using TMPro;

public class CoopStatus : MonoBehaviour
{
    private const string FILE = "Interface";
    private const float LOADING_RATE = 0.5f;
    
    [SerializeField] private LocalizationBase localization;
    [SerializeField] private TextMeshProUGUI text;
    private Coroutine coroutine;
    
    public enum Mode
    {
        None,
        Local,
        InvalidIP,
        CreationError,
        Created,
        Connecting,
        PlayerConnecting,
        ConnectionError,
        PasswordWrong,
        FullSession,
        Network
    }

    public void SetMode(Mode mode)
    {
        if (coroutine != null) StopCoroutine(coroutine);

        var phrase = Utilities.ToCamelCase(mode.ToString());
        localization.UpdatePhrase(FILE, phrase);
        
        switch (mode)
        {
            case Mode.Local:
            case Mode.Network:
            case Mode.Created:
                text.color = Settings.Brown;
                break;
            
            case Mode.Connecting:
            case Mode.PlayerConnecting:
                StartConnecting();
                break;
            
            case Mode.InvalidIP:
            case Mode.CreationError:
            case Mode.ConnectionError:
            case Mode.PasswordWrong:
            case Mode.FullSession:
                ShowError();
                break;
        }
    }

    private void StartConnecting()
    {
        text.color = Settings.Brown;
        coroutine = StartCoroutine(Connecting());
    }
    
    private IEnumerator Connecting()
    {
        int dotCount = 0;
        float timer = 0;
        
        while (timer < Settings.NETWORK_CONNECTING_TIMER)
        {
            if (dotCount > 2)
            {
                text.text = text.text.Remove(text.text.Length - dotCount,dotCount);
                dotCount = 0;
            }
            else
            {
                text.text += ".";
                dotCount++;
            }

            timer += LOADING_RATE;
            yield return new WaitForSeconds(LOADING_RATE);
        }
        
        coroutine = null;
    }
    
    private void ShowError()
    {
        text.color = Settings.Red;
        coroutine = StartCoroutine(HideError());
    }
    
    private IEnumerator HideError()
    {
        var color = text.color;

        yield return new WaitForSeconds(3);

        while (color.a > 0)
        {
            color.a -= 0.001f;
            text.color = color;
            yield return null;
        }

        coroutine = null;

        if (Settings.GameMode is GameMode.Host)
        {
            SetMode(Mode.Created);
        }
    }
}