using UnityEngine;

public class Room : ProgressObjectWithCounter
{
    [Header("Room")]
    [SerializeField] private ProgressObjectsManager targets;
    [SerializeField] private Door[] doors;

    protected override void Start()
    {
        target = targets.GetTargetCount();
    }

    public override void OnReset()
    {
        base.OnReset();
        
        foreach (var door in doors)
        {
            door.Close();
        }
        
        targets.OnReset();
    }

    protected override void Done()
    {
        foreach (var door in doors)
        {
            door.Open();
        }
        
        base.Done();
    }
}