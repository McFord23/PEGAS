using Enums;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private GameObject fireball;

    private void Start()
    {
        var tempTransform = transform;
        fireball = tempTransform.gameObject;

        Invoke("AutoDestroy", 1.0f);
    }

    private void AutoDestroy()
    {
        DestroyFireball();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DestroyFireball();
    }

    private void DestroyFireball()
    {
        if (Global.gameMode != GameMode.Client)
        {
            if (fireball) Destroy(fireball);
        }
    }
}