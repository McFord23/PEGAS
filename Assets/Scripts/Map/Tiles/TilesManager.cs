using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TilesManager : MonoBehaviour, ITaskTargetCount
{
    public UnityEvent progressIncreaseEvent;
    public UnityEvent progressDecreaseEvent;

    public GameObject floor;
    public GameObject wallPrefab;
    public Transform wallsParent;
    public Transform[] tilesClusters;
    
    private readonly List<Tile> tiles = new();
    
    private void Start()
    {
        foreach (var tilesCluster in tilesClusters)
        {
            for (int i = 0; i < tilesCluster.childCount; i++)
            {
                var tile = tilesCluster.GetChild(i).GetComponent<Tile>();
                tile.Initialize(this);
                tiles.Add(tile);
            }
        }
    }

    public void OnReset()
    {
        foreach (var tile in tiles)
        {
            tile.OnReset();
        }
    }

    public int GetTargetCount()
    {
        var tilesAmount = 0;
        
        foreach (var tilesCluster in tilesClusters)
        {
            tilesAmount += tilesCluster.childCount;
        }

        return tilesAmount;
    }
    
    public void GenerateWalls()
    {
        var walls = new List<Vector3>();
        floor.SetActive(false);

        foreach (var tilesCluster in tilesClusters)
        {
            for (int i = 0; i < tilesCluster.childCount; i++)
            {
                var tile = tilesCluster.GetChild(i).GetComponent<Tile>();
                var size = tile.GetSize();
                var wallsPoints = tile.GetWallsPoints();

                foreach (var wallPoint in wallsPoints)
                {
                    if (walls.Contains(wallPoint)) continue;
                
                    var wall = Instantiate(wallPrefab, wallPoint, new Quaternion(0,0,0,1), wallsParent);
                    wall.transform.localScale = new Vector3(size, wall.transform.localScale.y, size);
                    walls.Add(wallPoint);
                }
            }
        }
        
        floor.SetActive(true);
    }
}