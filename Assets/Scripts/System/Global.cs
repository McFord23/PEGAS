using Enums;

public static class Global
{
    public static GameMode gameMode = GameMode.Single;
    public static bool fullParty = false;

    public struct Player
    {
        public Character character;
        public ControlLayout controlLayout;
        public int gamepad;
        public bool live;
    }

    public static Player[] players =
    {
        new()
        {
            character = Character.Celestia, 
            controlLayout = ControlLayout.Mouse,
            gamepad = 1,
            live = true
        },

        new()
        {
            character = Character.Luna,
            controlLayout = ControlLayout.Numpad,
            gamepad = 2,
            live = true
        }
    };
    
    public struct Sensitivity
    {
        public static float mouse = 2f;
        public static float keyboard = 6f;
        public static float gamepad = 6f;
    }
    
    public static bool sound = true;
    public static bool music = true;
}
