using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Rigidbody3DResetter
{
    private Rigidbody rigidbody;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private MonoBehaviour monoBehaviour;

    public Rigidbody3DResetter(Rigidbody rigidbody, MonoBehaviour monoBehaviour)
    {
        this.rigidbody = rigidbody;
        spawnPosition = rigidbody.position;
        spawnRotation = rigidbody.rotation;

        this.monoBehaviour = monoBehaviour;
    }
        
    public void Reset(bool teleportBack = true)
    {
        if (!rigidbody.isKinematic)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        if (teleportBack) monoBehaviour.StartCoroutine(TeleportBack());
    }

    private IEnumerator TeleportBack()
    {
        var constrains = rigidbody.constraints;
        rigidbody.constraints = RigidbodyConstraints.None;

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
        
        yield return new WaitForFixedUpdate();
        rigidbody.constraints = constrains;
    }
}