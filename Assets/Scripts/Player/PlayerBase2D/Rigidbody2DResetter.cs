using System;
using UnityEngine;

[Serializable]
public class Rigidbody2DResetter
{
    private Rigidbody2D rigidbody;
    private Vector2 spawnPosition;
    private float spawnRotation;

    public Rigidbody2DResetter(Rigidbody2D rigidbody)
    {
        this.rigidbody = rigidbody;
        spawnPosition = rigidbody.position;
        spawnRotation = rigidbody.rotation;
    }
        
    public void Reset(bool teleportBack = true)
    {
        if (!rigidbody.simulated)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = 0;
        }
        
        if (!teleportBack) return;

        if (rigidbody.simulated)
        {
            rigidbody.MovePosition(spawnPosition);
            rigidbody.MoveRotation(spawnRotation);
        }
        else
        {
            rigidbody.position = spawnPosition;
            rigidbody.rotation = spawnRotation;
        }
    }
}