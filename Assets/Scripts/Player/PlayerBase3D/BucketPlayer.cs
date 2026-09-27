using UnityEngine;
using UnityEngine.Serialization;

public class BucketPlayer : WallToWallPlayer
{
    [FormerlySerializedAs("info")]
    [Header("Bucket")]
    [SerializeField] private PlayerHint playerHint;
    
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
        if (!Controls.WasMainActionPressed(isPlayer1)) return;

        var newStatus = status is Status.Wet ? Status.Dry : Status.Wet;
        SetStatus(newStatus);
    }
    
    private void SetStatus(Status newStatus, bool init = false)
    {
        status = newStatus;
        var playerNumber = isPlayer1 ? 1 : 2;
        movementCollider.name = $"Player {playerNumber} {status.ToString()} Collider";
        
        var mopType = Utilities.ToCamelCase(status.ToString());
        var phrase= LocalizationManager.GetPhrase($"{LevelsManager.GetActiveLevel().ToString()}/Gameplay", $"{mopType}Mop");
        if (!init) playerHint.Show(phrase, Color.white);
    }
}