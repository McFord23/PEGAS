public static class Settings
{
    public static GameMode GameMode { get; set; } = GameMode.Single;
    public static bool FullParty { get; set; } = false;
    
    public static bool Sound { get; set; } = true;
    public static bool Music { get; set; } = true;
}

public enum GameMode
{
    Single,
    LocalCoop,
    Host,
    Client
}
