using UnityEngine;

public class BucketPlayer : WallToWallPlayer
{
    [Header("Bucket Player")]
    [SerializeField] private Transform collider;
    private Status status;
    
    private enum Status
    {
        Wet,
        Dry,
        Dirt
    }

    public override void Initialize(PlayersSettings.Player player, PlayersManager manager)
    {
        base.Initialize(player, manager);
        SetStatus(Status.Wet);
    }

    protected override void Update()
    {
        base.Update();
        
        if (!IsInputAvailable()) return;
        if (MainActionInput == 0) return;
        if (!Controls.MainActionPressed) return;

        var newStatus = status is Status.Wet ? Status.Dry : Status.Wet;
        SetStatus(newStatus);
    }
    
    private void SetStatus(Status newStatus)
    {
        status = newStatus;
        var playerNumber = isPlayer1 ? 1 : 2;
        collider.name = $"Player {playerNumber} {status.ToString()} Collider";
    }
}