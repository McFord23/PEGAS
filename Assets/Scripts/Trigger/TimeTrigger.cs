using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/**
 * Триггер, который срабатывает через определённое количество секунд
 */
public class TimeTrigger : MonoBehaviour
{
    [SerializeField] private float timer = 10;
    [SerializeField] private UnityEvent onTimeEndEvent;

    private Coroutine timerCoroutine;
    
    public void OnEnable()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(Timer());
    }

    public void OnReset()
    {
        if (timerCoroutine == null) return;
        
        StopCoroutine(timerCoroutine);
        timerCoroutine = null;
    }
    
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(timer);

        timerCoroutine = null;
        onTimeEndEvent?.Invoke();
    }
}