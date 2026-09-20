using System.Collections;
using UnityEngine;

public class WallToWallPlayer : PlayerBase3D
{
    private const Direction SPAWN_DIRECTION = Direction.Forward;
    private const float ANGLE_TOLERANCE = 1;
    private const float MAX_DISTANCE = 25;
    private const float COLLIDER_MIN_SIZE = 0.025f;
    private const float COLLIDER_SIZE_SPEED = 0.01f;

    [Header("Sprite Rotation")]
    [SerializeField] private Rigidbody spriteRigidbody;
    [SerializeField] private Rigidbody spriteJointRigidbody;
    
    [Header("Movement")]
    [SerializeField] protected CapsuleCollider movementCollider;
    [SerializeField] private SpringJoint movementJoint;
    [SerializeField] private Rigidbody movementTarget;
    [SerializeField] private LayerMask movementLayer;
    private RigidbodyConstraints idleConstraints;
    private float colliderMaxSize;
    private float springForce;
    
    [Header("Additional Camera")]
    [SerializeField] private GameObject closeCamera;
    private GameObject strategicCamera;
    
    private Coroutine lookAtCoroutine;
    private Coroutine colliderSizeCoroutine;
    
    protected Mode mode;
    protected Direction direction;
    
    protected enum Mode
    {
        Strategic,
        Close
    }
    
    protected enum Direction
    {
        Forward,
        Right,
        Back,
        Left
    }

    public override void Initialize(PlayersSettings.Player player, PlayersManager manager)
    {
        base.Initialize(player, manager);
        ChangeDirection(SPAWN_DIRECTION);
        AddRigidbodyResetter(movementTarget);
        AddRigidbodyResetter(spriteRigidbody);
        AddRigidbodyResetter(spriteJointRigidbody);

        idleConstraints = rigidbody.constraints;
        colliderMaxSize = movementCollider.radius;
        springForce = movementJoint.spring;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        var ratio = springForce / Vector3.Distance(rigidbody.position, movementTarget.position);
        movementJoint.spring = Mathf.Clamp(ratio, 0.5f, springForce);
        
        if (Speed == 0)
        {
            if (Mathf.Approximately(movementCollider.radius, COLLIDER_MIN_SIZE))
            {
                if (colliderSizeCoroutine != null) StopCoroutine(colliderSizeCoroutine);
                colliderSizeCoroutine = StartCoroutine(SetColliderSize(colliderMaxSize));
            }

            if (rigidbody.constraints is RigidbodyConstraints.FreezeAll)
            {
                rigidbody.constraints = idleConstraints;
            }
        }
        
        spriteJointRigidbody.Move(rigidbody.position, rigidbody.rotation);
    }
    
    protected virtual void Update()
    {
        if (!IsInputAvailable()) return;
        
        switch (mode)
        {
            case Mode.Strategic:
                StrategicRotate();
                break;
            
            case Mode.Close:
                CloseRotate();
                break;
        }
        
        //ChangeCamera();
    }

    public void SetStrategicCamera(GameObject camera)
    {
        strategicCamera = camera;
    }

    public override void OnReset(bool teleportBack = true)
    {
        base.OnReset(teleportBack);
        ChangeDirection(SPAWN_DIRECTION);
    }

    private void StrategicRotate()
    {
        if (MoveInput == Vector2.zero) return;
        if (!Controls.MovePressed) return;
        if (Speed > 0.1f) return;
        
        var newDirection = MoveInput.x switch
        {
            > 0 => Direction.Right,
            < 0 => Direction.Left,
            _ => MoveInput.y switch
            {
                > 0 => Direction.Forward,
                < 0 => Direction.Back,
                _ => direction
            }
        };

        RigidbodiesResetter[rigidbody].Reset(false);
        rigidbody.rotation = Quaternion.Euler(new Vector3(0, (int)newDirection * 90,0));
        ChangeDirection(newDirection);
        StartCoroutine(Move());
    }

    private void CloseRotate()
    {
        if (MoveInput == Vector2.zero) return;
        if (!Controls.MovePressed) return;
        if (Speed > 0.1f) return;
        
        float angle = 0;
        
        switch (MoveInput.x)
        {
            case > 0:
                angle = 90;
                break;
            
            case < 0:
                angle = -90;
                break;
            
            default:
                switch (MoveInput.y)
                {
                    case > 0:
                        StartCoroutine(Move());
                        return;
                    
                    case < 0:
                        angle = 180;
                        break;
                }
                break;
        }
        
        RigidbodiesResetter[rigidbody].Reset(false);
        var rotation = rigidbody.rotation.eulerAngles + new Vector3(0, angle,0);
        rigidbody.rotation = Quaternion.Euler(rotation);

        angle = rigidbody.rotation.eulerAngles.y;
        if (angle < 0) angle += 360;

        var newDirection = angle switch
        {
            >= 0 - ANGLE_TOLERANCE and <= 0 + ANGLE_TOLERANCE => Direction.Forward,
            >= 90 - ANGLE_TOLERANCE and <= 90 + ANGLE_TOLERANCE => Direction.Right,
            >= 180 - ANGLE_TOLERANCE and <= 180 + ANGLE_TOLERANCE => Direction.Back,
            >= 270 - ANGLE_TOLERANCE and <= 270 + ANGLE_TOLERANCE => Direction.Left,
            _ => direction
        };
        
        ChangeDirection(newDirection);
    }
    
    private IEnumerator Move()
    {
        yield return new WaitForFixedUpdate();
        
        var playerPosition = new Vector3(rigidbody.position.x, 0.2f, rigidbody.position.z);
        
        if (Physics.Raycast(playerPosition, rigidbody.transform.forward, out var wallHit, MAX_DISTANCE, movementLayer))
        {
            var downPoint = Vector3.MoveTowards(wallHit.point, playerPosition, 0.1f);
            
            if (Physics.Raycast(downPoint, Vector3.down, out var tileHit, 1, movementLayer))
            {
                var tileObject = tileHit.transform.gameObject;
                var tilePosition = tileObject.transform.position;
                
                movementTarget.constraints = RigidbodyConstraints.None;
                movementTarget.position = tilePosition;
                yield return new WaitForFixedUpdate();
                movementTarget.constraints = RigidbodyConstraints.FreezeAll;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                
                if (colliderSizeCoroutine != null) StopCoroutine(colliderSizeCoroutine);
                colliderSizeCoroutine = StartCoroutine(SetColliderSize(COLLIDER_MIN_SIZE));
            }
        }
    }
    
    private void ChangeDirection(Direction newDirection)
    {
        direction = newDirection;
        
        if (mode is Mode.Strategic)
        {
            animator.Play(direction.ToString());
        }
        
        if (lookAtCoroutine != null) StopCoroutine(lookAtCoroutine);
        lookAtCoroutine = StartCoroutine(SpriteLookAtCamera());
    }
    
    private void ChangeCamera()
    {
        if (Settings.GameMode is GameMode.LocalCoop) return;
        if (AdditionalActionInput == 0) return;
        if (!Controls.AdditionalActionPressed) return;
        
        if (mode is Mode.Strategic)
        {
            animator.Play(Direction.Forward.ToString());
            strategicCamera.SetActive(false);
            closeCamera.SetActive(true);
            mode = Mode.Close;
        }
        else
        {
            closeCamera.SetActive(false);
            strategicCamera.SetActive(true);
            animator.Play(direction.ToString());
            mode = Mode.Strategic;
        }
        
        if (lookAtCoroutine != null) StopCoroutine(lookAtCoroutine);
        lookAtCoroutine = StartCoroutine(SpriteLookAtCamera());
    }
    
    private IEnumerator SpriteLookAtCamera()
    {
        // задержка дабы rigidbody успел сперва повернуть
        yield return new WaitForFixedUpdate();
        
        var targetPosition = Vector3.zero;

        switch (mode)
        {
            case Mode.Strategic:
                targetPosition = new Vector3(animator.transform.position.x, animator.transform.position.y, strategicCamera.transform.position.z);
                break;
            
            case Mode.Close:
                targetPosition = new Vector3(animator.transform.localPosition.x, animator.transform.localPosition.y, closeCamera.transform.localPosition.z);
                targetPosition = transform.TransformPoint(targetPosition);
                break;
        }
        
        animator.transform.LookAt(targetPosition);
        lookAtCoroutine = null;
    }

    private IEnumerator SetColliderSize(float value)
    {
        var tempDirection = movementCollider.radius < value ? 1 : -1;

        while (Mathf.Approximately(movementCollider.radius, value))
        {
            movementCollider.radius += tempDirection * COLLIDER_SIZE_SPEED;
            yield return new WaitForFixedUpdate();
        }

        movementCollider.radius = value;
        colliderSizeCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gold;
        Gizmos.DrawCube(movementTarget.position, new Vector3(0.3f, 0.05f, 0.3f));
        Gizmos.DrawLine(rigidbody.position, movementTarget.position);
        Gizmos.DrawRay(rigidbody.position + Vector3.up * 0.3f, rigidbody.transform.forward);
    }
}