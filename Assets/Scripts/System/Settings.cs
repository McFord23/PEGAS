using UnityEngine;

public static class Settings
{
    public static GameMode GameMode { get; private set; } = GameMode.Single;
    public static bool Sound { get; set; } = true;
    public static bool Music { get; set; } = true;
    
    public const float NETWORK_CONNECTING_TIMER = 15f;
    public static readonly Color Brown = new (0.4509f,0.349f,0.137f);
    public static readonly Color Red = new (0.45f, 0.2f, 0.15f);
    
    public delegate void ChangeGameModeEvent();
    public static event ChangeGameModeEvent OnChangeGameModeEvent;

    public static void ChangeGameMode(GameMode mode)
    {
        GameMode = mode;
        OnChangeGameModeEvent?.Invoke();
    }
}

public enum GameMode
{
    Single,
    LocalCoop,
    Host,
    Client
}
