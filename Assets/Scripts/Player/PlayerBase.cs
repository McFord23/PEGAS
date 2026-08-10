using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerBase : NetworkBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public float MainActionInput { get; private set; }
    public float AdditionalActionInput { get; private set; }
    
    public bool Live { get; protected set; } = true;
    public float Speed { get; private set; }
    
    protected Rigidbody2D rigidbody;
    protected Animator animatorController;
    protected SoundController soundController;
    protected PlayersManager playersManager;

    private PlayerInput input;
    private Vector2 spawnPosition;
    private Vector2 savedDirection;
    
    protected virtual void Start()
    {
        spawnPosition = transform.position;
        rigidbody = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        animatorController = GetComponentInChildren<Animator>();
        soundController = SoundController.Instance;
        playersManager = PlayersManager.Instance;
        playersManager.LoadPlayer(this);
    }

    protected virtual void FixedUpdate()
    {
        Speed = rigidbody.linearVelocity.magnitude;
    }

    public virtual void Pause()
    {
        animatorController.speed = 0;
        
        savedDirection = rigidbody.linearVelocity;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        rigidbody.gravityScale = 0f;
    }

    public virtual void Resume()
    {
        animatorController.speed = 1;

        rigidbody.gravityScale = 1f;
        rigidbody.constraints = RigidbodyConstraints2D.None;
        rigidbody.AddForce(savedDirection * 500f, ForceMode2D.Impulse);
    }
    
    public virtual void Revive(bool teleportBack = true)
    {
        rigidbody.linearVelocity = new Vector2(0, 0);
        rigidbody.angularVelocity = 0f;
        
        if (teleportBack) rigidbody.position = spawnPosition;

        Live = true;
    }

    public virtual void Kill()
    {
        Live = false;
    }

    public virtual void Victory()
    {
        rigidbody.gravityScale = 0f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody.linearVelocity = new Vector2(0, 0);
    }
    
    public void SwitchControlScheme(ControlScheme controlScheme)
    {
        input.SwitchCurrentControlScheme(controlScheme.ToString(), InputSystem.devices.ToArray());
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

    private void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    private void OnMainAction(InputValue value)
    {
        MainActionInput = value.Get<float>();
    }

    private void OnAdditionalAction(InputValue value)
    {
        AdditionalActionInput = value.Get<float>();
    }
}
