using UnityEngine;

public class FlyingPlayer : PlayerBase
{
    [Header("Flying")]
    [SerializeField] private float wingPower = 10000f;
    [SerializeField] private float rotationRatio = 250f;
    [SerializeField] private float wingSpan = 6; // realistic 13.56f
    [SerializeField] private float wingArea = 22; // realistic 78.04f
    [SerializeField] private float takeoffForce = 250f;
    [SerializeField] private float takeoffSpeed = 40f;
    [SerializeField] private float maxLandedSpeed = 30f;
    [SerializeField] private float crushSpeed = 80f;
    
    [Header("Live")]
    [SerializeField] private float reviveTime = 3f;
    
    private PolygonCollider2D flyCollider;
    private CapsuleCollider2D deathCollider;
    private BoxCollider2D rideCollider;
    private CrushEffect crushEffect;
    private CollectingItem item;
    
    private float aspectRatio;
    private float angleOfAttack;
    private bool landed;
    
    private float reviveTimer;
    
    private MoveState moveState;
    private MoveState savedState;
    
    private enum MoveState
    {
        Idle,
        Run,
        FreeFall,
        Flap,
        Stunned
    }

    public override void Initialize(PlayersSettings.Player player)
    {
        base.Initialize(player);
        
        rigidbody.linearDamping = Mathf.Epsilon;
        aspectRatio = (wingSpan * wingSpan) / wingArea;
        flyCollider = GetComponentInChildren<PolygonCollider2D>();
        deathCollider = GetComponentInChildren<CapsuleCollider2D>();
        rideCollider = GetComponentInChildren<BoxCollider2D>();
        crushEffect = GetComponentInChildren<CrushEffect>();
        
        Idle();
    }

    private void Update()
    {
        if (!IsInputAvailable()) return;
        
        switch (moveState)
        {
            case MoveState.Stunned:
                if (Live) UpdateReviveTime();
                break;
            
            case MoveState.Run:
                if (MainActionInput == 0) Idle();
                break;
            
            case MoveState.Flap:
                if (MainActionInput == 0) FreeFall();
                break;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (Settings.GameMode is GameMode.Host or GameMode.Client && !IsOwner)
        {
            return;
        }

        switch (moveState)
        {
            case MoveState.Idle or MoveState.Run:
                RidePhysics();
                if (Speed >= takeoffSpeed) SetFlyMode(true);
                break;
            
            case MoveState.FreeFall or MoveState.Flap:
                FlyPhysics();
                break;
            
            case MoveState.Stunned:
                rigidbody.linearDamping = landed ? 3f : 0.3f;
                break;
                
        }

        if (IsInputAvailable())
        {
            RotateInput();
            AccelerationInput();
        }
        
        TryFlip();
    }
    
    private void UpdateReviveTime()
    {
        reviveTimer -= Time.deltaTime;
        if (!(reviveTimer <= 0)) return;
        
        Revive(false);
    }

    private void RotateInput()
    {
        var playerSettings = Settings.GameMode == GameMode.Client ? PlayersSettings.Player2 : PlayersSettings.Player1;
        var rotateInput = (MoveInput.y - MoveInput.x) * playerSettings.Sensitivity * rotationRatio;

        switch (moveState)
        {
            case MoveState.Idle or MoveState.Run:
                rigidbody.AddTorque(rotateInput * Speed / rigidbody.mass);
                break;
            
            case MoveState.Flap or MoveState.FreeFall:
                rigidbody.AddTorque(rotateInput);
                break;
        }
    }

    private void AccelerationInput()
    {
        var accelerationInput = MainActionInput * wingPower;

        if (accelerationInput > 0)
        {
            switch (moveState)
            {
                case MoveState.Idle or MoveState.Run:
                    Run(accelerationInput);
                    break;
            
                case MoveState.Flap or MoveState.FreeFall:
                    Flap(accelerationInput);
                    break;
            }
        }
    }
    
    public override void Revive(bool teleportBack = true)
    {
        base.Revive(teleportBack);

        if (teleportBack)
        {
            transform.right = Vector2.right;
            transform.localScale = new Vector3(1, 1, 1);
        }
        
        rigidbody.linearDamping = Mathf.Epsilon;
        rigidbody.angularDamping = 2.5f;
        
        deathCollider.enabled = false;
        rideCollider.enabled = true;
        Idle();

        if (item)
        {
            item.ExecuteDrop();
            item = null;
        }
    }

    public override void Victory()
    {
        base.Victory();
        animatorController.Play("FreeFall");
    }

    public override void Pause()
    {
        base.Pause();
        savedState = moveState;
    }

    public override void Resume()
    {
        base.Resume();
        moveState = savedState;
    }

    private void Idle()
    {
        moveState = MoveState.Idle;
        animatorController.Play("Idle");
    }

    private void Run(float torque)
    {
        moveState = MoveState.Run;
        animatorController.Play("Walk");

        var gear = (Speed < 20) ? 1.5f : 1;
        if (landed) rigidbody.AddForce(transform.right * (gear * torque));
    }

    private void RidePhysics()
    {
        var friction = (landed) ? -0.3f : 0f;
        if (moveState == MoveState.Idle) friction = -rigidbody.mass;

        var velocity = rigidbody.linearVelocity;
        var drag = 0.021f * velocity.sqrMagnitude;
        var dragDirection = -velocity.normalized;

        rigidbody.AddForce(dragDirection * drag + velocity * friction);
    }

    private void FreeFall()
    {
        moveState = MoveState.FreeFall;
        animatorController.Play("FreeFall");
    }

    private void Flap(float flapForce)
    {
        moveState = MoveState.Flap;
        animatorController.Play("Flap");

        var gear = (Speed < 20) ? 2 : 1;
        rigidbody.AddForce(transform.right * (gear * flapForce));
    }

    private void FlyPhysics()
    {
        var velocity = rigidbody.linearVelocity;
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
        rigidbody.AddForce(liftDirection * lift + dragDirection * drag);
    }

    private void TryFlip()
    {
        if (rigidbody.linearVelocityX > 0.01f && transform.localScale.y < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (rigidbody.linearVelocityX < -0.01f && transform.localScale.y > 0)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
    }
    
    private void SetFlyMode(bool fly)
    {
        rideCollider.enabled = !fly;
        flyCollider.enabled = fly;

        if (fly)
        {
            FreeFall();
            rigidbody.AddForce(Vector2.up * takeoffForce);
        }
        else
        {
            rigidbody.angularVelocity = 0;
            Idle();
        }
    }
    
    public override void Kill()
    {
        moveState = MoveState.Stunned;
        deathCollider.enabled = true;
        flyCollider.enabled = false;
        animatorController.Play("Dead");
        rigidbody.gravityScale = 1f;
        rigidbody.angularDamping = 0.3f;
            
        if (Speed >= crushSpeed)
        {
            Live = false;
            crushEffect.StartExplode();
            playersManager.ExecuteDeath();
        }
        else
        {
            reviveTimer = reviveTime;
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
        
        if (!Live)
        {
            soundController.Hit();
        }
        else if (moveState is MoveState.FreeFall or MoveState.Flap)
        {
            if (Speed <= maxLandedSpeed)
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
       
        if (moveState is MoveState.Idle or MoveState.Run && Speed > 1)
        {
            SetFlyMode(true);
        }
    }

   private void OnTriggerEnter2D(Collider2D collider)
   {
       if (!collider.gameObject.CompareTag("Item") || item || !Live) return;
       var tempItem = collider.gameObject.GetComponent<CollectingItem>();
            
       if (!tempItem.owner)
       {
           item = tempItem;
           item.ExecutePickUp(transform);
       }
   }
}