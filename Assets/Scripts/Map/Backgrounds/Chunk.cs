using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Background background;
    private bool playerOnRight;

    public void Start()
    {
        background = GetComponentInParent<Background>();
        if (background.transparency < 1) SetTransparency();
    }

    private void SetTransparency()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var material = transform.GetChild(i).GetComponent<Renderer>().material;
            var color = material.color;
            color.a = background.transparency;
            material.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("MainCamera"))
        {
            var playerPos = transform.InverseTransformPoint(collider.transform.position);
            playerOnRight = playerPos.x > background.size / 2;

            background.LoadChunk(transform, playerOnRight);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("MainCamera"))
        {
            var playerPos = transform.InverseTransformPoint(collider.transform.position);
            
            if (playerOnRight == playerPos.x > background.size / 2)
            {
                background.LoadChunk(transform, !playerOnRight);
            }
        }
    }
}
