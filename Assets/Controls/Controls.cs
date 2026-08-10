using UnityEngine;

public class Controls : MonoBehaviour
{
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
}
