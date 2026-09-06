using UnityEngine.InputSystem;

public static class PlayersSettings
{
    public static bool IsSharedGamepad { get; private set; }
    
    public class Player
    {
        public PlayerCharacter Character;
        public ControlScheme ControlScheme;
        public float Sensitivity;
        public Gamepad Gamepad;
        public bool NetworkGamepad;
    }

    public static Player Player1 { get; } = new()
    {
        Character = PlayerCharacter.Celestia,
        ControlScheme = ControlScheme.WASD,
        Sensitivity = 0.5f,
        Gamepad = null,
        NetworkGamepad = false
    };
    
    public static Player Player2 { get; } = new()
    {
        Character = PlayerCharacter.Luna,
        ControlScheme = ControlScheme.Arrows,
        Sensitivity = 0.5f,
        Gamepad = null,
        NetworkGamepad = false
    };
    
    public delegate void SwapCharactersEvent();
    public static event SwapCharactersEvent OnSwapCharactersEvent;

    public static void ClearSubscribers()
    {
        OnSwapCharactersEvent = null;
    }
    
    public static void SwapCharacters()
    {
        (Player1.Character, Player2.Character) = (Player2.Character, Player1.Character);
        OnSwapCharactersEvent?.Invoke();
    }
    
    public static bool CanShareGamepad()
    {
        var isLocalCoop = Settings.GameMode is GameMode.LocalCoop;
        var isZeroGamepad = Player1.Gamepad == null && Player2.Gamepad == null; 
        var isTwoGamepad = Player1.Gamepad != null && Player2.Gamepad != null && !IsSharedGamepad;

        return isLocalCoop && !isZeroGamepad && !isTwoGamepad;
    }
    
    public static void ShareGamepad(bool fromPlayer1)
    {
        if (!CanShareGamepad()) return;
        
        if (fromPlayer1)
        {
            Player2.Gamepad = Player1.Gamepad;
        }
        else
        {
            Player1.Gamepad = Player2.Gamepad;
        }

        IsSharedGamepad = true;
    }

    public static void GiveGamepad(bool toPlayer1)
    {
        if (!CanShareGamepad()) return;
        
        IsSharedGamepad = false;
        
        if (toPlayer1)
        {
            Player1.Gamepad = Player2.Gamepad;
            Player2.Gamepad = null;
        }
        else
        {
            Player2.Gamepad = Player1.Gamepad;
            Player1.Gamepad = null;
        }
    }
}
