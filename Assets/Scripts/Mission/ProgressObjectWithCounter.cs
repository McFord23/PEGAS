using UnityEngine;
using UnityEngine.Events;

public class ProgressObjectWithCounter : ProgressObject
{
    public int target;

    [SerializeField] private UnityEvent doneEvent;
    
    private bool isDone;
    private int progress;
    
    public override void OnReset()
    {
        progress = 0;
        isDone = false;
    }

    public override void ChangeProgress(bool isIncrease)
    {
        base.ChangeProgress(isIncrease);

        if (isIncrease)
        {
            if (isDone) return;
        
            progress++;
        
            if (progress < target) return;

            Done();
        }
        else if (progress > 0)
        {
            progress--;
        }
    }

    protected virtual void Done()
    {
        isDone = true;
        doneEvent?.Invoke();
    }
}