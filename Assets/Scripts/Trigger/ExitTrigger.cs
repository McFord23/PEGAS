using UnityEngine;
using UnityEngine.Events;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent exitEvent;
    
    private void OnTriggerExit2D(Collider2D other)
    {
        exitEvent?.Invoke();
    }
    
    private void OnTriggerExit(Collider collider)
    {
        exitEvent?.Invoke();
    }
}