using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private const float HEIGHT = 0.1f;
    
    [SerializeField] private float size = 0.3f;
    [SerializeField] private LayerMask mask;

    protected TilesManager tilesManager;
    
    public virtual void Initialize(TilesManager manager)
    {
        tilesManager = manager;
    }
    
    public virtual void OnReset() { }

    public float GetSize()
    {
        return size;
    }
    
    public List<Vector3> GetWallsPoints()
    {
        var wallsPoints = new List<Vector3>();
        CheckFloor(Vector3.forward, wallsPoints);
        CheckFloor(Vector3.right, wallsPoints);
        CheckFloor(Vector3.back, wallsPoints);
        CheckFloor(Vector3.left, wallsPoints);
        return wallsPoints;
    }

    private void CheckFloor(Vector3 direction, List<Vector3> points)
    {
        var origin = transform.position + (Vector3.up * HEIGHT) + (direction * size);
        var distance = size + HEIGHT;

        if (Physics.Raycast(origin, -transform.up, distance, mask)) return;
        
        points.Add( transform.position + (direction * size) );
    }
}