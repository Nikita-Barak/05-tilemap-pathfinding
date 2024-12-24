﻿using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This component allows the player to move by clicking the arrow keys,
 * but only if the new position is on an allowed tile.
 */
public class KeyboardMoverByTile: KeyboardMover {
    [SerializeField] protected Tilemap tilemap = null;
//    [SerializeField] TileBase[] allowedTiles = null;
    [SerializeField] protected AllowedTiles allowedTiles = null;

    protected TileBase TileOnPosition(Vector3 worldPosition) {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return tilemap.GetTile(cellPosition);
    }

    virtual protected void Update()  {
        Vector3 newPosition = NewPosition();
        TileBase tileOnNewPosition = TileOnPosition(newPosition);
        if (allowedTiles.Contains(tileOnNewPosition)) {
            transform.position = newPosition;
        } else {
            Debug.LogError("You cannot walk on " + tileOnNewPosition + "!");
        }
    }
}
