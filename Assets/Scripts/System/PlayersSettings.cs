using UnityEngine;

public static class PlayersSettings
{
    public const float SENSITIVITY_MIN = 0.1f;
    public const float SENSITIVITY_MAX = 1f;
    
    public class PlayerSettings
    {
        public Character Character;
        public ControlLayout ControlLayout;
        public float Sensitivity;
        public bool Gamepad;
    }

    public static PlayerSettings Player1 { get; } = new()
    {
        Character = Character.Celestia,
        ControlLayout = ControlLayout.Mouse,
        Sensitivity = 0.5f,
        Gamepad = false
    };
    
    public static PlayerSettings Player2 { get; } = new()
    {
        Character = Character.Luna,
        ControlLayout = ControlLayout.Numpad,
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
    
public enum ControlLayout
{
    Mouse,
    Numpad,
    WASD,
    IJKL,
    Arrow
}
