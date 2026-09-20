using UnityEngine;

public class BucketPlayer : WallToWallPlayer
{
    [Header("Bucket")]
    [SerializeField] private Info info;
    
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
        SetStatus(Status.Wet, true);
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
    
    private void SetStatus(Status newStatus, bool init = false)
    {
        status = newStatus;
        var playerNumber = isPlayer1 ? 1 : 2;
        movementCollider.name = $"Player {playerNumber} {status.ToString()} Collider";
        
        var mopType = Utilities.ToCamelCase(status.ToString());
        var phrase= LocalizationManager.GetPhrase("Gameplay", $"{mopType}Mop");
        if (!init) info.Show(phrase, Color.white);
    }
}