using UnityEngine;

public class Room : ProgressObject
{
    public int Target => targets.GetTargetCount();
    
    [SerializeField] private ProgressObjectsManager targets;
    [SerializeField] private Door[] doors;
    
    private bool isDone;
    private int progress;

    public override void OnReset()
    {
        foreach (var door in doors)
        {
            door.Close();
        }

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
        
            if (progress < Target) return;

            isDone = true;
        
            foreach (var door in doors)
            {
                door.Open();
            }
        }
        else if (progress > 0)
        {
            progress--;
        }
    }
}