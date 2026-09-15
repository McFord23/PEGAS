using System;
using UnityEngine;

[Serializable]
public class Rigidbody3DResetter
{
    private Rigidbody rigidbody;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    public Rigidbody3DResetter(Rigidbody rigidbody)
    {
        this.rigidbody = rigidbody;
        spawnPosition = rigidbody.position;
        spawnRotation = rigidbody.rotation;
    }
        
    public void Reset(bool teleportBack = true)
    {
        if (!rigidbody.isKinematic)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        
        if (!teleportBack) return;

        if (rigidbody.isKinematic)
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