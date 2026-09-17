using UnityEngine;

public class DirtTile : Tile
{
    [SerializeField] private Material dirtMaterial;
    [SerializeField] private Material wetMaterial;
    
    private Material cleanMaterial;
    private MeshRenderer meshRenderer;

    private Status status;
    
    private enum Status
    {
        Dirt,
        Wet,
        Clean
    }
    
    public override void Initialize(TilesManager manager)
    {
        base.Initialize(manager);
        meshRenderer = GetComponent<MeshRenderer>();
        cleanMaterial = meshRenderer.material;
        SetStatus(Status.Dirt);
    }

    public override void OnReset()
    {
        SetStatus(Status.Dirt);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        switch (other.name)
        {
            case "Wet Collider":
                if (status is Status.Clean)
                {
                    tilesManager.progressDecreaseEvent.Invoke();
                }
                
                SetStatus(Status.Wet);
                break;
            
            case "Dry Collider":
                if (status is Status.Wet)
                {
                    SetStatus(Status.Clean);
                    tilesManager.progressIncreaseEvent.Invoke();
                }
                break;
            
            case "Dirt Collider":
                if (status is Status.Clean)
                {
                    SetStatus(Status.Dirt);
                    tilesManager.progressDecreaseEvent.Invoke();
                }
                break;
        }
    }

    private void SetStatus(Status newStatus)
    {
        status = newStatus;
        meshRenderer.material = status switch
        {
            Status.Dirt => dirtMaterial,
            Status.Wet => wetMaterial,
            _ => cleanMaterial
        };
    }
}