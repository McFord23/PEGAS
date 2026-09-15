using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsForwardEnd { get; private set; }
    public bool IsRightEnd { get; private set; }
    public bool IsBackEnd { get; private set; }
    public bool IsLeftEnd { get; private set; }

    [SerializeField] private float size = 0.3f;
    [SerializeField] private LayerMask mask;
    
    public void CheckWalls()
    {
        IsForwardEnd = HasWall(Vector3.forward);
        IsRightEnd = HasWall(Vector3.right);
        IsBackEnd = HasWall(Vector3.back);
        IsLeftEnd = HasWall(Vector3.left);
    }

    private bool HasWall(Vector3 direction)
    {
        var origin = transform.position + Vector3.up * 0.1f;
        var distance = size + 0.1f;
        
        return Physics.Raycast(origin, direction, distance, mask);
    }

    private void OnDrawGizmosSelected()
    {
        var origin = transform.position + Vector3.up * 0.1f;
        var distance = size + 0.1f;

        Gizmos.color = Color.green;
        if (IsForwardEnd) Gizmos.DrawRay(origin, Vector3.forward * distance);
        if (IsRightEnd) Gizmos.DrawRay(origin, Vector3.right * distance);
        if (IsBackEnd) Gizmos.DrawRay(origin, Vector3.back * distance);
        if (IsLeftEnd) Gizmos.DrawRay(origin, Vector3.left * distance);
    }
}