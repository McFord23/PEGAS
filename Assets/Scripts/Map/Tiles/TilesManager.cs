using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TilesManager : MonoBehaviour
{
    public UnityEvent progressIncreaseEvent;
    public UnityEvent progressDecreaseEvent;
    
    private readonly List<Tile> tiles = new();
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var tile = transform.GetChild(i).GetComponent<Tile>();
            tile.Initialize(this);
            tiles.Add(tile);
        }
    }

    public void OnReset()
    {
        foreach (var tile in tiles)
        {
            tile.OnReset();
        }
    }
}