using System.Collections.Generic;
using UnityEngine;

public class PlayerBase2D : PlayerBase
{
    [SerializeField] protected Rigidbody2D rigidbody2D;
    private Dictionary<Rigidbody2D, Rigidbody2DResetter> RigidbodiesResetter { get; } = new();
    
    private Vector2 savedVelocity2D;
    private RigidbodyConstraints2D savedConstraints2D;

    public override void Initialize(PlayersSettings.Player player, PlayersManager manager)
    {
        base.Initialize(player, manager);
        AddRigidbodyResetter(rigidbody2D);
        
        savedConstraints2D = rigidbody2D.constraints;
    }

    public override Vector3 GetPosition()
    {
        return rigidbody2D.position;
    }

    protected virtual void FixedUpdate()
    {
        Speed = rigidbody2D.linearVelocity.magnitude;
    }
    
    public override void OnReset(bool teleportBack = true)
    {
        base.OnReset(teleportBack);
        
        foreach (var rigidbodyResetter in RigidbodiesResetter)
        {
            rigidbodyResetter.Value.Reset(teleportBack);
        }
    }

    protected void AddRigidbodyResetter(Rigidbody2D rb)
    {
        RigidbodiesResetter.Add(rb, new Rigidbody2DResetter(rb, this));
    }
    
    protected override void Freeze()
    {
        savedVelocity2D = rigidbody2D.linearVelocity;
        savedConstraints2D = rigidbody2D.constraints;
            
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.linearVelocity = Vector3.zero;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    protected override void UnFreeze()
    {
        rigidbody2D.gravityScale = 1f;
        rigidbody2D.linearVelocity = savedVelocity2D;
        rigidbody2D.constraints = savedConstraints2D;
    }
}
