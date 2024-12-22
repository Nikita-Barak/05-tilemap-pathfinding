using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This component moves its object towards a given target position.
 */
public class TargetMover : MonoBehaviour {
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] AllowedTilesWithCost allowedTiles = null;

    [Tooltip("The speed by which the object moves towards the target, in meters (=grid units) per second")]
    [SerializeField] float baseSpeed = 2f;

    [Tooltip("Maximum number of iterations before BFS/A* algorithm gives up on finding a path")]
    [SerializeField] int maxIterations = 1000;

    [Tooltip("The target position in world coordinates")]
    [SerializeField] Vector3 targetInWorld;

    [Tooltip("The target position in grid coordinates")]
    [SerializeField] Vector3Int targetInGrid;

    protected bool atTarget;

    public void SetTarget(Vector3 newTarget) {
        if (targetInWorld != newTarget) {
            targetInWorld = newTarget;
            targetInGrid = tilemap.WorldToCell(targetInWorld);
            atTarget = false;
        }
    }

    public Vector3 GetTarget() {
        return targetInWorld;
    }

    private TilemapGraph tilemapGraph = null;

    protected virtual void Start() {
        tilemapGraph = new TilemapGraph(tilemap, allowedTiles.Get(), allowedTiles.GetPrices());
        StartCoroutine(MoveTowardsTheTarget());
    }

    IEnumerator MoveTowardsTheTarget() {
        while (true) {
            if (enabled && !atTarget) {
                yield return MakeOneStepTowardsTheTarget();
            } else {
                yield return null;
            }
        }
    }

    IEnumerator MakeOneStepTowardsTheTarget() {
        Vector3Int startNode = tilemap.WorldToCell(transform.position);
        Vector3Int endNode = targetInGrid;
        List<Vector3Int> shortestPath = A_STAR.GetPath(tilemapGraph, startNode, endNode, maxIterations);

        if (shortestPath.Count >= 2) {
            Vector3Int nextNode = shortestPath[1];
            float moveSpeed = GetSpeedForTile(nextNode);

            // Move smoothly to the target tile
            Vector3 nextPosition = tilemap.GetCellCenterWorld(nextNode);
            while (Vector3.Distance(transform.position, nextPosition) > 0.01f) {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    nextPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Snap to the target tile to avoid floating-point errors
            transform.position = nextPosition;
        } else {
            if (shortestPath.Count == 0) {
                Debug.LogError($"No path found between {startNode} and {endNode}");
            }
            atTarget = true;
        }
    }

    private float GetSpeedForTile(Vector3Int tilePosition) {
        TileBase tile = tilemap.GetTile(tilePosition);
        if (tile != null && allowedTiles.Contains(tile)) {
            return baseSpeed / allowedTiles.GetCost(tile); // Adjust speed based on tile cost
        }
        return baseSpeed; // Default speed for undefined tiles
    }
}