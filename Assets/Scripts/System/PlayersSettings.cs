using UnityEngine.InputSystem;

public static class PlayersSettings
{
    public class Player
    {
        public Character Character;
        public ControlScheme ControlScheme;
        public float Sensitivity;
        public Gamepad Gamepad;
        public bool NetworkGamepad;
    }

    public static Player Player1 { get; } = new()
    {
        Character = Character.Celestia,
        ControlScheme = ControlScheme.Mouse,
        Sensitivity = 0.5f,
        Gamepad = null,
        NetworkGamepad = false
    };
    
    public static Player Player2 { get; } = new()
    {
        Character = Character.Luna,
        ControlScheme = ControlScheme.WASD,
        Sensitivity = 0.5f,
        Gamepad = null,
        NetworkGamepad = false
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
    Numpad
}
