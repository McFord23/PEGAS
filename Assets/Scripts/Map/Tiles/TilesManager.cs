using System.Collections.Generic;
using UnityEngine;

public class TilesManager : ProgressObjectsManager
{
    [Header("Tile")]
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private Transform wallsParent;

    public static class Fields
    {
        public const string PROGRESS_OBJECTS = nameof(progressObjects);
        public const string PROGRESS_INCREASE_EVENT = nameof(progressIncreaseEvent);
        public const string PROGRESS_DECREASE_EVENT = nameof(progressDecreaseEvent);
        
        public const string FLOOR = nameof(floor);
        public const string WALL_PREFAB = nameof(wallPrefab);
        public const string WALLS_PARENT = nameof(wallsParent);
    }

    public void AssignTiles()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var tile = transform.GetChild(i).GetComponent<ProgressObject>();
            progressObjects.Add(tile);
        }
    }
    
    public void GenerateWalls()
    {
        var walls = new List<Vector3>();
        floor.SetActive(false);

        for (int i = 0; i < transform.childCount; i++)
        {
            var tile = transform.GetChild(i).GetComponent<Tile>();
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
        
        floor.SetActive(true);
    }
}