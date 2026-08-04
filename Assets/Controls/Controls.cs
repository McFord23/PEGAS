using UnityEngine;

public class Controls : MonoBehaviour
{
    // Gameplay
    public static Vector2 Move => inputData.Gameplay.Move.ReadValue<Vector2>();
    public static float MainAction => inputData.Gameplay.MainAction.ReadValue<float>();
    public static float AdditionalAction => inputData.Gameplay.AdditionalAction.ReadValue<float>();

    // UI
    public static bool Retry => inputData.UI.Retry.triggered;
    public static bool Pause => inputData.UI.Pause.triggered;
    public static bool Apply => inputData.UI.Apply.triggered;
    public static bool Paste => inputData.UI.Paste.triggered;
    public static Vector2 Navigation => inputData.UI.Navigation.ReadValue<Vector2>();
    
    // DevOps
    public static bool Logs => inputData.DevOps.Logs.triggered;
    
    private static InputData inputData;
    
    private void Awake()
    {
        inputData = new InputData();
        inputData.Enable();
    }
    
    private void OnDestroy()
    {
        inputData.Disable();
    }
}
