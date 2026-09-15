using UnityEngine;

public class SnowfallTrigger : MonoBehaviour
{
    Snowfall rainController;

    void Start()
    {
        rainController = GetComponent<Snowfall>();
    }
 
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            rainController.Active(true);
        }
    } 

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            rainController.Active(false);
        }
    }
}
