using UnityEngine;

public class RoomsManager : ProgressObjectsManager
{
    public override int GetTargetCount()
    {
        var targets = 0;
        
        foreach (var progressObject in progressObjects)
        {
            var room = progressObject as Room;
            
            if (room == null)
            {
                Debug.LogError($"Assigned ProjectObject {progressObject.name} isn't room");
                continue;
            }
            
            targets += room.Target;
        }

        return targets;
    }
}