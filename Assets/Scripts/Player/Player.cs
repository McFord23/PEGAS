using Unity.Netcode;
using UnityEngine;

public enum MoveState
{
    Loaded,
    Idle,
    Run,
    FreeFall,
    Flap,
    Stunned,
    Dead,
    Paused,
    Winner
}

public class Player : NetworkBehaviour
{
    private const float MaxLandedSpeed = 30f;
    private const float TakeoffSpeed = 40f;
    private const float TakeoffForce = 250f;
    private const float CrushSpeed = 80f;
    private const float ReviveTime = 3f;

    [SerializeField] private GameObject crushEffectPrefab;

    public bool Live { get; private set; } = true;
    
    public float speed { private set; get; }
    private float angleOfAttack;
    private bool landed;

    public Vector2 spawnPos;

    private Vector2 saveDirection;
    private MoveState saveState;

    public Rigidbody2D rb { get; private set; }
    private PolygonCollider2D flyCollider;
    private CapsuleCollider2D deathCollider;
    private BoxCollider2D rideCollider;

    public MoveState moveState;
    
    private Animator animatorController;
    private SoundController soundController;
    private PlayersManager playersManager;

    private CollectingItem item;

    // Fly Physics
    [SerializeField] private float wingSpan = 13.56f;
    [SerializeField] private float wingArea = 78.04f;
    private float aspectRatio;
    
    private float reviveTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = Mathf.Epsilon;
        aspectRatio = (wingSpan * wingSpan) / wingArea;
        flyCollider = GetComponentInChildren<PolygonCollider2D>();
        deathCollider = GetComponentInChildren<CapsuleCollider2D>();
        rideCollider = GetComponentInChildren<BoxCollider2D>();
        
        playersManager = PlayersManager.Instance;
        playersManager.LoadPlayer(this);
        spawnPos = playersManager.transform.position;
        
        soundController = SoundController.Instance;
        animatorController = GetComponentInChildren<Animator>();
        
        Idle();
    }

    private void Update()
    {
        if (Settings.GameMode is GameMode.Host or GameMode.Client)
        {
            if (!IsOwner) return;
        }

        if (moveState == MoveState.Stunned)
        {
            UpdateReviveTime();
        }
    }

    private void FixedUpdate()
    {
        if (Settings.GameMode is GameMode.Host or GameMode.Client)
        {
            if (!IsOwner) return;
        }

        if (moveState is MoveState.Idle or MoveState.Run)
        {
            RidePhysics();

            if (speed >= TakeoffSpeed)
            {
                SetFlyMode(true);
            }
        }

        if (moveState is MoveState.Flap or MoveState.FreeFall)
        {
            FlyPhysics();
        }

        if (moveState is MoveState.Stunned or MoveState.Dead)
        {
            rb.linearDamping = landed ? 3f : 0.3f;
        }

        HandleFlyFlip();

        speed = rb.linearVelocity.magnitude;
    }
    
    private void UpdateReviveTime()
    {
        reviveTimer -= Time.deltaTime;
        if (!(reviveTimer <= 0)) return;
        
        Revive(false);
    }

    public void Revive(bool teleportBack = true)
    {
        rb.linearVelocity = new Vector2(0, 0);
        rb.linearDamping = Mathf.Epsilon;
        rb.angularDamping = 2.5f;
        rb.angularVelocity = 0f;
        
        var localTransform = transform;
        if (teleportBack) rb.position = spawnPos;
        localTransform.right = Vector2.right;
        localTransform.localScale = new Vector3(1, 1, 1);
        
        deathCollider.enabled = false;
        rideCollider.enabled = true;
        Idle();

        if (item)
        {
            item.ExecuteDrop();
            item = null;
        }
        
        Live = true;
    }

    public void Victory()
    {
        moveState = MoveState.Winner;
        animatorController.Play("FreeFall");

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = new Vector2(0, 0);
    }

    public void Pause()
    {
        saveState = moveState;
        saveDirection = rb.linearVelocity;

        moveState = MoveState.Paused;
        animatorController.speed = 0;

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Resume()
    {
        moveState = saveState;
        animatorController.speed = 1;

        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(saveDirection * 500, ForceMode2D.Impulse);
    }

    public void Idle()
    {
        moveState = MoveState.Idle;
        animatorController.Play("Idle");
    }

    public void Run(float torque)
    {
        moveState = MoveState.Run;
        animatorController.Play("Walk");

        var gear = (speed < 20) ? 1.5f : 1;
        if (landed) rb.AddForce(transform.right * (gear * torque));
    }

    private void RidePhysics()
    {
        var friction = (landed) ? -0.3f : 0f;
        if (moveState == MoveState.Idle) friction = -rb.mass;

        var velocity = rb.linearVelocity;
        var drag = 0.021f * velocity.sqrMagnitude;
        var dragDirection = -velocity.normalized;

        rb.AddForce(dragDirection * drag + velocity * friction);
    }

    public void FreeFall()
    {
        moveState = MoveState.FreeFall;
        animatorController.Play("FreeFall");
    }

    public void Flap(float flapForce)
    {
        moveState = MoveState.Flap;
        animatorController.Play("Flap");

        var gear = (speed < 20) ? 2 : 1;
        rb.AddForce(transform.right * (gear * flapForce));
    }

    private void FlyPhysics()
    {
        var velocity = rb.linearVelocity;
        var localVelocity = transform.InverseTransformDirection(velocity);
        angleOfAttack = Mathf.Atan2(localVelocity.y, localVelocity.x);

        var inducedLift = angleOfAttack * (aspectRatio / (aspectRatio + 2f)) * 2f * Mathf.PI;
        var inducedDrag = (inducedLift * inducedLift) / (aspectRatio * Mathf.PI);
        var pressure = velocity.sqrMagnitude * 1.2754f * 0.5f * wingArea;
        var lift = inducedLift * pressure;
        var drag = (0.021f + inducedDrag) * pressure;

        var dragDirection = -(Vector3)velocity.normalized;
        var liftDirection = Vector3.Cross(dragDirection, -transform.forward);

        // Lift + Drag = Total Force
        rb.AddForce(liftDirection * lift + dragDirection * drag);
    }

    private void HandleFlyFlip()
    {
        var localTransform = transform;
        var scaleY = rb.linearVelocity.x switch
        {
            > 0.0001f => 1,
            < -0.0001f => -1,
            _ => localTransform.localScale.y
        };
        localTransform.localScale = new Vector3(1, scaleY, 1);
    }
    
    private void SetFlyMode(bool fly)
    {
        rideCollider.enabled = !fly;
        flyCollider.enabled = fly;

        if (fly)
        {
            FreeFall();
            rb.AddForce(Vector2.up * TakeoffForce);
        }
        else
        {
            rb.angularVelocity = 0;
            Idle();
        }
    }
    
    public void Kill()
    {
        deathCollider.enabled = true;
        flyCollider.enabled = false;
        animatorController.Play("Dead");
        rb.gravityScale = 1f;
        rb.angularDamping = 0.3f;
            
        if (speed >= CrushSpeed)
        {
            Live = false;
            Instantiate(crushEffectPrefab, transform.position, Quaternion.identity);
            moveState = MoveState.Dead;
            playersManager.Dead();
        }
        else
        {
            moveState = MoveState.Stunned;
            reviveTimer = ReviveTime;
        }

        if (item)
        {
            item.ExecuteDrop();
            item = null;
        }
    }
    
    private void OnCollisionEnter2D()
    {
        landed = true;
        
        if (moveState is MoveState.Dead)
        {
            soundController.Hit();
        }
        else if (moveState is MoveState.FreeFall or MoveState.Flap)
        {
            if (speed <= MaxLandedSpeed)
            {
                SetFlyMode(false);
                return;
            }
        
            soundController.Hit();
            Kill();
        }
    }

   private void OnCollisionExit2D()
   {
       landed = false;
        
        if (moveState is MoveState.Idle or MoveState.Run)
        {
            SetFlyMode(true);
        }
    }

   private void OnTriggerEnter2D(Collider2D collider)
   {
       if (!collider.gameObject.CompareTag("Item") || item || moveState == MoveState.Dead) return;
       var tempItem = collider.gameObject.GetComponent<CollectingItem>();
            
       if (!tempItem.owner)
       {
           item = tempItem;
           item.ExecutePickUp(transform);
       }
   }
}