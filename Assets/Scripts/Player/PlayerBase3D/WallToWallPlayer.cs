using System.Collections;
using UnityEngine;

public class WallToWallPlayer : PlayerBase3D
{
    private const Direction SPAWN_DIRECTION = Direction.Forward;
    private const float ANGLE_TOLERANCE = 1;
    private const float MAX_DISTANCE = 25;

    [Header("Sprite Rotation")]
    [SerializeField] private Rigidbody spriteRigidbody;
    [SerializeField] private Rigidbody spriteJointRigidbody;
    [SerializeField] private float spriteRotationSpeed = 0.1f;
    [SerializeField] private Transform rotationTarget;
    
    [Header("Movement")]
    [SerializeField] private Rigidbody movementTarget;
    [SerializeField] private LayerMask movementLayer;
    
    [Header("Additional Camera")]
    [SerializeField] private GameObject closeCamera;
    private GameObject strategicCamera;
    
    private Coroutine lookAt;
    private Coroutine move;
    
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
        AddRigidbodyResetor(movementTarget);
        AddRigidbodyResetor(spriteRigidbody);
        AddRigidbodyResetor(spriteJointRigidbody);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
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
        
        ChangeCamera();
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
        if (move != null) return;
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
        var rotation = rigidbody.rotation;
        rotation.eulerAngles = new Vector3(0, (int)newDirection * 90,0);
        rigidbody.rotation = rotation;
        
        ChangeDirection(newDirection);
        
        if (move != null) StopCoroutine(move);
        move = StartCoroutine(Move());
    }

    private void CloseRotate()
    {
        if (MoveInput == Vector2.zero) return;
        if (!Controls.MovePressed) return;
        if (move != null) return;
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
                        if (move != null) StopCoroutine(move);
                        move = StartCoroutine(Move());
                        return;
                    
                    case < 0:
                        angle = 180;
                        break;
                }
                break;
        }
        
        RigidbodiesResetter[rigidbody].Reset(false);
        var rotation = rigidbody.rotation;
        rotation.eulerAngles += new Vector3(0, angle,0);
        rigidbody.rotation = rotation;

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
            if (Physics.Raycast(wallHit.point, Vector3.down, out var tileHit, 1, movementLayer))
            {
                var tileObject = tileHit.transform.gameObject;
                var tilePosition = tileObject.transform.position;

                RigidbodiesResetter[movementTarget].Reset(false);
                movementTarget.position = tilePosition;
            }
        }
        
        move = null;
    }
    
    private void ChangeDirection(Direction newDirection)
    {
        direction = newDirection;
        
        if (mode is Mode.Strategic)
        {
            animator.Play(direction.ToString());
        }
        
        if (lookAt != null) StopCoroutine(lookAt);
        lookAt = StartCoroutine(SpriteLookAtCamera());
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
        
        if (lookAt != null) StopCoroutine(lookAt);
        lookAt = StartCoroutine(SpriteLookAtCamera());
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

        rotationTarget.LookAt(targetPosition);
        var startRotation = animator.transform.rotation;
        float progress = 0;
        
        while (progress < 1)
        {
            animator.transform.rotation = Quaternion.Lerp(startRotation, rotationTarget.rotation, progress);
            progress += spriteRotationSpeed;
            
            yield return new WaitForFixedUpdate();
        }

        animator.transform.rotation = rotationTarget.rotation;
        
        lookAt = null;
    }
}