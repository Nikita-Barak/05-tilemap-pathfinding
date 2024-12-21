using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AllowedTilesWithCost : AllowedTiles {
    [System.Serializable]
    public class TileData {
        public TileBase tile;     // The tile type
        public float cost = 1f;  // Movement cost
    }

    [SerializeField] private List<TileData> tileDataWithCosts = new List<TileData>();

    private Dictionary<TileBase, float> tileCostMap;

    private void Awake() {
        // Build a dictionary for fast lookups
        tileCostMap = new Dictionary<TileBase, float>();
        foreach (var tileData in tileDataWithCosts) {
            tileCostMap[tileData.tile] = tileData.cost;
        }
    }

    public float GetCost(TileBase tile) {
        return tileCostMap.ContainsKey(tile) ? tileCostMap[tile] : float.MaxValue; // High cost for disallowed tiles
    }

    public new TileBase[] Get() {
        // Merge functionality: Return both allowed tiles and tiles with costs
        var baseTiles = base.Get();
        var tilesWithCosts = tileDataWithCosts.ConvertAll(t => t.tile);
        return baseTiles.Union(tilesWithCosts).ToArray();
    }
    
    public List<float> GetPrices() {
        var tiles = Get(); // Retrieve the TileBase array from the parent class
        return tiles.Select(tile => tileDataWithCosts.First(t => t.tile == tile).cost).ToList(); // ensure that list order of original get() and getPrices() are matching
    }

    public new bool Contains(TileBase tile) {
        // Check both the original allowed tiles and tiles with costs
        return base.Contains(tile) || tileCostMap.ContainsKey(tile);
    }
}