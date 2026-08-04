using Enums;

public static class Global
{
    public static GameMode gameMode = GameMode.Single;
    public static bool fullParty = false;

    public struct Player
    {
        public Character character;
        public ControlLayout controlLayout;
        public float sensitivity;
        public int gamepad;
        public bool live;
    }

    public static readonly Player[] players =
    {
        new()
        {
            character = Character.Celestia, 
            controlLayout = ControlLayout.Mouse,
            sensitivity = 0.5f,
            gamepad = 1,
            live = true
        },

        new()
        {
            character = Character.Luna,
            controlLayout = ControlLayout.Numpad,
            sensitivity = 0.5f,
            gamepad = 2,
            live = true
        }
    };
    
    public static bool sound = true;
    public static bool music = true;
}
