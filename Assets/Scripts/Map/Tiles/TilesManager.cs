using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    private readonly List<Tile> tiles = new();
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var tile = transform.GetChild(i).GetComponent<Tile>();
            tiles.Add(tile);
        }
    }
}