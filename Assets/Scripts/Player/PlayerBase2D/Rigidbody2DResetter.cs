using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Rigidbody2DResetter
{
    private Rigidbody2D rigidbody;
    private Vector2 spawnPosition;
    private float spawnRotation;

    private MonoBehaviour monoBehaviour;

    public Rigidbody2DResetter(Rigidbody2D rigidbody, MonoBehaviour monoBehaviour)
    {
        this.rigidbody = rigidbody;
        spawnPosition = rigidbody.position;
        spawnRotation = rigidbody.rotation;

        this.monoBehaviour = monoBehaviour;
    }
        
    public void Reset(bool teleportBack = true)
    {
        if (rigidbody.simulated)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = 0;
        }

        if (teleportBack) monoBehaviour.StartCoroutine(TeleportBack());
    }
    
    private IEnumerator TeleportBack()
    {
        var constrains = rigidbody.constraints;
        rigidbody.constraints = RigidbodyConstraints2D.None;

        rigidbody.position = spawnPosition;
        rigidbody.rotation = spawnRotation;
        
        yield return new WaitForFixedUpdate();
        rigidbody.constraints = constrains;
    }
}