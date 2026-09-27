using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProgressObjectsManager : MonoBehaviour
{
    [SerializeField] protected List<ProgressObject> progressObjects;
    [SerializeField] protected UnityEvent progressIncreaseEvent;
    [SerializeField] protected UnityEvent progressDecreaseEvent;
    
    protected virtual void Start()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.OnChangeProgressEvent += OnChangeProgress;
        }
    }
    
    private void OnDestroy()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.OnChangeProgressEvent -= OnChangeProgress;
        }
    }
    
    public void OnReset()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.OnReset();
        }
    }
    
    public int GetTargetCount()
    {
        if (progressObjects[0] is not ProgressObjectWithCounter) return progressObjects.Count;
        
        var targets = 0;
            
        foreach (var progressObject in progressObjects)
        {
            var objectWithCounter = progressObject as ProgressObjectWithCounter;
            if (objectWithCounter == null) continue;
            targets += objectWithCounter.target;
        }

        return targets;
    }
    
    private void OnChangeProgress(bool isIncrease)
    {
        if (isIncrease) progressIncreaseEvent?.Invoke();
        else progressDecreaseEvent?.Invoke();
    }
}