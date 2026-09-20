using System.Collections.Generic;
using UnityEngine;

public class PlayerBase3D : PlayerBase
{
    [SerializeField] protected Rigidbody rigidbody;
    protected Dictionary<Rigidbody, Rigidbody3DResetter> RigidbodiesResetter { get; } = new();
    
    private Vector3 savedVelocity;
    private RigidbodyConstraints savedConstraints;
    
    public override void Initialize(PlayersSettings.Player player, PlayersManager manager)
    {
        base.Initialize(player, manager);
        AddRigidbodyResetter(rigidbody);

        savedConstraints = rigidbody.constraints;
    }
    
    public override Vector3 GetPosition()
    {
        return rigidbody.position;
    }

    protected virtual void FixedUpdate()
    {
        Speed = rigidbody.linearVelocity.magnitude;
    }
    
    public override void OnReset(bool teleportBack = true)
    {
        base.OnReset(teleportBack);

        foreach (var rigidbodyResetter in RigidbodiesResetter)
        {
            rigidbodyResetter.Value.Reset(teleportBack);
        }
    }

    protected void AddRigidbodyResetter(Rigidbody rb)
    {
        RigidbodiesResetter.Add(rb, new Rigidbody3DResetter(rb, this));
    }
    
    protected override void Freeze()
    {
        savedVelocity = rigidbody.linearVelocity;
        savedConstraints = rigidbody.constraints;
            
        rigidbody.useGravity = false;
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    protected override void UnFreeze()
    {
        rigidbody.useGravity = true;
        rigidbody.constraints = savedConstraints;
        rigidbody.linearVelocity = savedVelocity;
    }
}