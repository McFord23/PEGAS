using UnityEngine;

public class Controls : MonoBehaviour
{
    private const float GAMEPAD_SENSITIVITY = 8;
    
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
    
    private void OnDestroy()
    {
        inputData.Disable();
    }
    
    public static Vector2 MoveByGamepad(PlayersSettings.Player player)
    {
        if (PlayersSettings.IsSharedGamepad)
        {
            if (player == PlayersSettings.Player1)
            {
                return player.Gamepad != null 
                    ? player.Gamepad.leftStick.ReadValue() * GAMEPAD_SENSITIVITY 
                    : Vector2.zero;
            }
            
            return player.Gamepad != null 
                ? player.Gamepad.rightStick.ReadValue() * GAMEPAD_SENSITIVITY 
                : Vector2.zero;
        }
        
        return player.Gamepad != null 
            ? player.Gamepad.leftStick.ReadValue() * GAMEPAD_SENSITIVITY 
            : Vector2.zero;
    }

    public static float MainActionByGamepad(PlayersSettings.Player player)
    {
        if (PlayersSettings.IsSharedGamepad)
        {
            if (player == PlayersSettings.Player1)
            {
                return player.Gamepad != null 
                    ? player.Gamepad.leftTrigger.ReadValue()  
                    : 0;
            }
            
            return player.Gamepad != null 
                ? player.Gamepad.rightTrigger.ReadValue()  
                : 0;
        }
        
        return player.Gamepad != null 
            ? player.Gamepad.leftTrigger.ReadValue()  
            : 0;
    }
    
    public static float AdditionalActionByGamepad(PlayersSettings.Player player)
    {
        if (PlayersSettings.IsSharedGamepad)
        {
            if (player == PlayersSettings.Player1)
            {
                return player.Gamepad != null 
                    ? player.Gamepad.leftShoulder.ReadValue()  
                    : 0;
            }
            
            return player.Gamepad != null 
                ? player.Gamepad.rightShoulder.ReadValue()  
                : 0;
        }
        
        return player.Gamepad != null 
            ? player.Gamepad.rightTrigger.ReadValue()  
            : 0;
    }
}
