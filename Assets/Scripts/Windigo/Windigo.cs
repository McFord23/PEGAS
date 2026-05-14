using UnityEngine;

public class Windigo : MonoBehaviour
{
    public float speed;

    // Fly Physics
    public float wingSpan = 13.56f;
    public float wingArea = 78.04f;
    private float aspectRatio;

    private Vector2 saveDirection;
    private MoveState saveState;

    private Rigidbody2D rb;
    private MoveState moveState = MoveState.Alive;

    public enum MoveState
    {
        Alive,
        Dead,
        Paused
    }

    private CollectingItem item;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = Mathf.Epsilon;
        aspectRatio = (wingSpan * wingSpan) / wingArea;
    }

    private void FixedUpdate()
    {
        if (moveState == MoveState.Alive)
        {
            FlyPhysics();
            Flap(speed);
        }
    }

    public void Pause()
    {
        saveState = moveState;
        saveDirection = rb.velocity;
        moveState = MoveState.Paused;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Resume()
    {
        moveState = saveState;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(saveDirection * 100, ForceMode2D.Impulse);
    }

    private void Flap(float flapForce)
    {
        float gear = (speed < 20) ? 2 : 1;
        rb.AddForce(transform.right * (gear * flapForce));
    }

    private void FlyPhysics()
    {
        var velocity = rb.velocity;
        var localVelocity = transform.InverseTransformDirection(velocity);
        var angleOfAttack = Mathf.Atan2(localVelocity.y, localVelocity.x);

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

    private void Dead()
    {
        gameObject.layer = 8;
        moveState = MoveState.Dead;

        if (item)
        {
            item.ExecuteDrop();
            item = null;
        }
    }

    public void Destroy(bool ignoreDeath = false)
    {
        if (moveState == MoveState.Dead || ignoreDeath)
        {
            WindigoDestructor.Instance.Destruct(this, ignoreDeath);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Dead();
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