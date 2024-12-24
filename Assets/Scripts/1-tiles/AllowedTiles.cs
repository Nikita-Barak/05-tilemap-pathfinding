using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This component just keeps a list of allowed tiles.
 * Such a list is used both for pathfinding and for movement.
 */
public class AllowedTiles : MonoBehaviour  {
    [SerializeField] TileBase[] allowedTiles = null;
    [SerializeField] TileBase[] waterTiles = null;
    [SerializeField] TileBase[] mountainTiles = null;

    public bool isOnShip = false;
    public bool isOnHorse = false;
    public bool hasPickaxe = false;

    public bool Contains(TileBase tile)
    {
        if(allowedTiles.Contains(tile))
        {
            return true;
        }
        else if(isOnShip)
        {
            return isWater(tile);
        }
        else if(isOnHorse || hasPickaxe)
        {
            return isMountain(tile);
        }
        return false;
    }

    public bool isWater(TileBase tile)
    {
        return waterTiles.Contains(tile);
    }

    public bool isMountain(TileBase tile)
    {
        return mountainTiles.Contains(tile);
    }

    public void ToggleIndicator(bool indicator)
    {
        indicator = !indicator;
    }

    public TileBase[] Get() { return allowedTiles;  }
}
