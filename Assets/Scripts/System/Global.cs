public static class Global
{
    public static bool IsPause { get; set; }
    public static bool IsLoading { get; set; }
    public static bool IsNetworkPlayerConnected { get; set; }

    public static int Player1Points { get; set; }
    public static int Player2Points { get; set; }

    public static void Reset()
    {
        IsPause = false;
        Player1Points = 0;
        Player2Points = 0;
    }
}
