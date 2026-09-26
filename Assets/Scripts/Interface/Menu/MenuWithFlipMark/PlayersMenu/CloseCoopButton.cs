using UnityEngine;
using UnityEngine.Events;

public class CloseCoopButton : MonoBehaviour
{
    // Back to coop submenu
    [SerializeField] private UnityEvent backEvent;
    
    // Local coop
    [SerializeField] private UnityEvent localKickEvent;
    
    // Network coop
    [SerializeField] private UnityEvent cancelEvent;
    [SerializeField] private UnityEvent quickEvent;
    [SerializeField] private UnityEvent shutdownEvent;
    
    private UnityEvent currentEvent;

    public enum Mode
    {
        Back,
        
        LocalKick,
        
        Cancel,
        Quick,
        ShutDown
    }

    private void Start()
    {
        var level = LevelManager.GetActiveLevel();
        if (LevelManager.IsLevelRequiresCoop(level) && Settings.GameMode is not GameMode.Single)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPressed()
    {
        currentEvent?.Invoke();
    }
    
    public void ChangeMode(Mode mode)
    {
        currentEvent = mode switch
        {
            Mode.Back => backEvent,
            
            Mode.LocalKick => localKickEvent,
            
            Mode.Cancel => cancelEvent,
            Mode.Quick => quickEvent,
            Mode.ShutDown => shutdownEvent,
            
            _ => currentEvent
        };
    }
}