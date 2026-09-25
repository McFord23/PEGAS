using UnityEngine;

public class DirtTile : Tile
{
    [SerializeField] private MeshRenderer dirt;
    [SerializeField] private Material dirtMaterial;
    [SerializeField] private Material wetMaterial;

    private Status status;
    
    private enum Status
    {
        Dirt,
        Wet,
        Clean
    }
    
    private void Start()
    {
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
            case "Player 1 Wet Collider":
            case "Player 2 Wet Collider":
                if (status is Status.Clean)
                {
                    ChangeProgress(false);

                    if (other.name.Contains("1")) Global.Player1Points--;
                    else Global.Player2Points--;
                }
                
                SetStatus(Status.Wet);
                break;
            
            case "Player 1 Dry Collider":
            case "Player 2 Dry Collider":
                if (status is Status.Wet)
                {
                    SetStatus(Status.Clean);
                    ChangeProgress(true);
                    
                    if (other.name.Contains("1")) Global.Player1Points++;
                    else Global.Player2Points++;
                }
                break;
            
            case "Player 1 Dirt Collider":
            case "Player 2 Dirt Collider":
                if (status is Status.Clean)
                {
                    SetStatus(Status.Dirt);
                    ChangeProgress(false);
                    
                    if (other.name.Contains("1")) Global.Player1Points--;
                    else Global.Player2Points--;
                }
                break;
        }
    }

    private void SetStatus(Status newStatus)
    {
        status = newStatus;

        switch (status)
        {
            case Status.Dirt:
                dirt.material = dirtMaterial;
                if (!dirt.gameObject.activeSelf)
                {
                    dirt.gameObject.SetActive(true);
                }
                break;
            
            case Status.Wet:
                dirt.material = wetMaterial;
                if (!dirt.gameObject.activeSelf)
                {
                    dirt.gameObject.SetActive(true);
                }
                break;
            
            case Status.Clean:
                dirt.gameObject.SetActive(false);
                break;
        }
    }
}