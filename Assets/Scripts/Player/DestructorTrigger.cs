using Enums;
using UnityEngine;

public class DestructorTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Global.gameMode is not (GameMode.Host or GameMode.Client)) return;
        
        if (collider.gameObject.CompareTag("Windigo"))
        {
            WindigoDestructor.Instance.UpdateSeeStatus();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Windigo"))
        {
            collider.gameObject.GetComponent<Windigo>().Destroy();
        }
    }
}