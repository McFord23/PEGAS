public static class PlayersSettings
{
    public class PlayerSettings
    {
        public Character Character;
        public ControlScheme ControlScheme;
        public float Sensitivity;
        public bool Gamepad;
    }

    public static PlayerSettings Player1 { get; } = new()
    {
        Character = Character.Celestia,
        ControlScheme = ControlScheme.Mouse,
        Sensitivity = 0.5f,
        Gamepad = false
    };
    
    public static PlayerSettings Player2 { get; } = new()
    {
        Character = Character.Luna,
        ControlScheme = ControlScheme.WASD,
        Sensitivity = 0.5f,
        Gamepad = false
    };

    public static void SwapCharacters()
    {
        (Player1.Character, Player2.Character) = (Player2.Character, Player1.Character);
    }
}

public enum Character
{
    Celestia,
    Luna
}
    
public enum ControlScheme
{
    Mouse,
    WASD,
    Arrows,
    Numpad,
    Gamepad
}
