using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * A graph that represents a tilemap, using only the allowed tiles.
 */
public class TilemapGraph: IGraph<Vector3Int> {
    private Tilemap tilemap;
    private TileBase[] allowedTiles;
    private float[] tilePrices; // New array for tile prices

    public TilemapGraph(Tilemap tilemap, TileBase[] allowedTiles) {
        this.tilemap = tilemap;
        this.allowedTiles = allowedTiles;
    }
    
    // New constructor to also include prices
    public TilemapGraph(Tilemap tilemap, TileBase[] allowedTiles, List<float> tilePrices) {
        this.tilemap = tilemap;
        this.allowedTiles = allowedTiles;
        this.tilePrices = tilePrices.ToArray();
    }

    static Vector3Int[] directions = {
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 1, 0),
    };

    public IEnumerable<Vector3Int> Neighbors(Vector3Int node) {
        foreach (var direction in directions) {
            Vector3Int neighborPos = node + direction;
            TileBase neighborTile = tilemap.GetTile(neighborPos);
            if (allowedTiles.Contains(neighborTile))
                yield return neighborPos;
        }
    }
    
    public float GetCost(Vector3Int from, Vector3Int to) {
        TileBase tile = tilemap.GetTile(to);
        int index = System.Array.IndexOf(allowedTiles, tile); // Find index of tile in allowedTiles
        return index >= 0 ? tilePrices[index] : float.MaxValue; // Return cost or a high cost for invalid tiles
    }
}
