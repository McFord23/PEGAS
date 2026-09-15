using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerBase : NetworkBehaviour
{
    public Vector2 MoveInput => moveByKeyboardAndMouse
                                + Controls.MoveByGamepad(playerSettings);
    public float MainActionInput => mainActionByKeyboardAndMouse 
                                    + Controls.MainActionByGamepad(playerSettings);

    public float AdditionalActionInput => additionalActionByKeyboardAndMouse 
                                          + Controls.AdditionalActionByGamepad(playerSettings);
    
    public bool Live { get; protected set; } = true;
    public float Speed { get; protected set; }
    
    [SerializeField] protected Animator animator;
    
    protected PlayersManager playersManager;
    
    private PlayersSettings.Player playerSettings;
    
    private PlayerInput input;
    private Vector2 moveByKeyboardAndMouse;
    private float mainActionByKeyboardAndMouse;
    private float additionalActionByKeyboardAndMouse;
    
    public virtual void Initialize(PlayersSettings.Player player, PlayersManager manager)
    {
        playerSettings = player;
        playersManager = manager;
        input = GetComponent<PlayerInput>();
        UpdateControlScheme();
    }

    public virtual void Pause()
    {
        Freeze();
        animator.speed = 0;
    }

    public virtual void Resume()
    {
        animator.speed = 1;
        UnFreeze();
    }
    
    public virtual void OnReset(bool teleportBack = true)
    {
        Live = true;
    }

    public virtual void Kill()
    {
        Live = false;
    }

    public virtual void Victory()
    {
        Freeze();
    }
    
    public void UpdateControlScheme()
    {
        input.SwitchCurrentControlScheme(playerSettings.ControlScheme.ToString(), InputSystem.devices.ToArray());
    }
    
    public bool IsInputAvailable()
    {
        if (!Live) return false;
        if (Global.IsPause) return false;
        
        if (Settings.GameMode is GameMode.Host or GameMode.Client && !IsOwner)
        {
            return false;
        }

        return true;
    }
    
    protected virtual void Freeze() {}
    
    protected virtual void UnFreeze() {}

    private void OnMove(InputValue value)
    {
        moveByKeyboardAndMouse = value.Get<Vector2>();
    }

    private void OnMainAction(InputValue value)
    {
        mainActionByKeyboardAndMouse = value.Get<float>();
    }

    private void OnAdditionalAction(InputValue value)
    {
        additionalActionByKeyboardAndMouse = value.Get<float>();
    }
}
