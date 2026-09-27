using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    public const float INPUT_DEAD_ZONE = 0.25f;
    private const float FLYING_SENSITIVITY = 8;
    
    // UI
    public static bool Retry => inputData.UI.Retry.triggered;
    public static bool Pause => inputData.UI.Pause.triggered;
    public static bool Apply => inputData.UI.Apply.triggered;
    public static bool Paste => inputData.UI.Paste.triggered;
    public static Vector2 Navigation => inputData.UI.Navigation.ReadValue<Vector2>();
    
    // DevOps
    public static bool Logs => inputData.DevOps.Logs.triggered;
    
    private static InputData inputData;
    
    private void OnEnable()
    {
        inputData = new InputData();
        inputData.Enable();
    }
    
    private void OnDisable()
    {
        inputData.Disable();
    }

    public static bool WasMovePressed(bool isPlayer1)
    {
        var keyboardAndMousePressed = inputData.Gameplay.Move.WasPressedThisFrame();
        bool gamepadPressed;

        if (PlayersSettings.IsSharedGamepad)
        {
            var sharedGamepad = isPlayer1 
                ? inputData.GameplaySharedGamepad.MoveP1 
                : inputData.GameplaySharedGamepad.MoveP2;
            
            gamepadPressed = sharedGamepad.WasPressedThisFrame();
        }
        else
        {
            gamepadPressed = inputData.GameplayGamepad.Move.WasPressedThisFrame();
        }
        
        return keyboardAndMousePressed || gamepadPressed;
    }
    
    public static bool WasMainActionPressed(bool isPlayer1)
    {
        var keyboardAndMousePressed = inputData.Gameplay.MainAction.WasPressedThisFrame();
        bool gamepadPressed;

        if (PlayersSettings.IsSharedGamepad)
        {
            var sharedGamepad = isPlayer1 
                ? inputData.GameplaySharedGamepad.MainActionP1 
                : inputData.GameplaySharedGamepad.MainActionP2;
            
            gamepadPressed = sharedGamepad.WasPressedThisFrame();
        }
        else
        {
            gamepadPressed = inputData.GameplayGamepad.MainAction.WasPressedThisFrame();
        }
        
        return keyboardAndMousePressed || gamepadPressed;
    }
    
    public static bool WasAdditionalActionPressed(bool isPlayer1)
    {
        var keyboardAndMousePressed = inputData.Gameplay.AdditionalAction.WasPressedThisFrame();
        bool gamepadPressed;

        if (PlayersSettings.IsSharedGamepad)
        {
            var sharedGamepad = isPlayer1 
                ? inputData.GameplaySharedGamepad.AdditionalActionP1 
                : inputData.GameplaySharedGamepad.AdditionalActionP2;
            
            gamepadPressed = sharedGamepad.WasPressedThisFrame();
        }
        else
        {
            gamepadPressed = inputData.GameplayGamepad.AdditionalAction.WasPressedThisFrame();
        }
        
        return keyboardAndMousePressed || gamepadPressed;
    }
    
    public static Vector2 MoveByGamepad(PlayersSettings.Player player)
    {
        var level = LevelsManager.GetActiveLevel();
        var sensitivity = LevelsManager.IsFlyingLevel(level) ? FLYING_SENSITIVITY : 1;
        
        if (PlayersSettings.IsSharedGamepad)
        {
            var inputAction = player == PlayersSettings.Player1
                ? inputData.GameplaySharedGamepad.MoveP1
                : inputData.GameplaySharedGamepad.MoveP2;
            
            return inputAction.ReadValue<Vector2>() * sensitivity;
        }
        
        return player.Gamepad != null 
            ? player.Gamepad.leftStick.ReadValue() * sensitivity
            : Vector2.zero;
    }

    public static float MainActionByGamepad(PlayersSettings.Player player)
    {
        if (PlayersSettings.IsSharedGamepad)
        {
            var inputAction = player == PlayersSettings.Player1
                ? inputData.GameplaySharedGamepad.MainActionP1
                : inputData.GameplaySharedGamepad.MainActionP2;
            
            return inputAction.ReadValue<float>();
        }
        
        return player.Gamepad != null 
            ? player.Gamepad.leftTrigger.ReadValue()  
            : 0;
    }
    
    public static float AdditionalActionByGamepad(PlayersSettings.Player player)
    {
        if (PlayersSettings.IsSharedGamepad)
        {
            var inputAction = player == PlayersSettings.Player1
                ? inputData.GameplaySharedGamepad.AdditionalActionP1
                : inputData.GameplaySharedGamepad.AdditionalActionP2;
            
            return inputAction.ReadValue<float>();
        }
        
        return player.Gamepad != null 
            ? player.Gamepad.rightTrigger.ReadValue()  
            : 0;
    }
}
