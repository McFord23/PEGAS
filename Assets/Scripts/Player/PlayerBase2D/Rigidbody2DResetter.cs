using System;
using UnityEngine;

[Serializable]
public class Rigidbody2DResetter
{
    private Rigidbody2D rigidbody;
    private Vector2 spawnPosition;
    private float spawnRotation;
    private Quaternion spawnRotationQuaternion;

    public Rigidbody2DResetter(Rigidbody2D rigidbody)
    {
        this.rigidbody = rigidbody;
        spawnPosition = rigidbody.position;
        spawnRotation = rigidbody.rotation;
        spawnRotationQuaternion = rigidbody.transform.rotation;
    }
        
    public void Reset(bool teleportBack = true)
    {
        if (rigidbody.simulated)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = 0;
        }
        
        if (!teleportBack) return;

        if (rigidbody.simulated)
        {
            rigidbody.position = spawnPosition;
            rigidbody.rotation = spawnRotation;
        }
        else
        {
            rigidbody.transform.position = spawnPosition;
            rigidbody.transform.rotation = spawnRotationQuaternion;
        }
    }
}