using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerVehicleTraversal : KeyboardMoverByTile
{
    private SpriteRenderer spriteRenderer; // Player's sprite renderer
    [SerializeField] private Sprite normalSprite; // Player's base sprite

    // Ship Variables
    [SerializeField] private Sprite shipSprite;
    [SerializeField] private TileBase shipTile;
    [SerializeField] private TileBase shallowSeaTile; // New shallow sea tile

    // Horse Variables
    [SerializeField] private Sprite horseSprite;
    [SerializeField] private TileBase horseTile; 
    [SerializeField] private TileBase mountainTile; // Horse tile replacement

    // Pickaxe Variables
    [SerializeField] private TileBase pickaxeTile;
    [SerializeField] private TileBase grassTile; // Grass that's left after using pickaxe on mountain tiles.
    private bool usingPicaxe; // Used mainly for preventing bugs when riding horse.

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        Vector3 newPosition = NewPosition();
        TileBase tileOnNewPosition = TileOnPosition(newPosition);

        // Check if the new tile is allowed using the Contains() method from AllowedTiles
        if (allowedTiles.Contains(tileOnNewPosition))
        {
            HandleVehicleTraversal(newPosition, tileOnNewPosition);
            HandlePickaxe(newPosition, tileOnNewPosition); // Handle pickaxe interaction
            transform.position = newPosition;
        }
        else
        {
            Debug.LogError("You cannot walk on " + tileOnNewPosition + "!");
        }
    }

    private void HandleVehicleTraversal(Vector3 newPosition, TileBase tileOnNewPosition)
    {
        // If the player is on the ship or horse and tries to leave water or mountain terrain
        if (allowedTiles.isOnShip || allowedTiles.isOnHorse)
        {
            // Check if the player is leaving the water or the terrain appropriate for the horse
            if (!allowedTiles.isWater(tileOnNewPosition) && !allowedTiles.isMountain(tileOnNewPosition))
            {
                // Drop the vehicle at the current position the player is leaving
                Vector3Int currentPosition = tilemap.WorldToCell(transform.position);
                SetTileAtPosition(currentPosition, allowedTiles.isOnShip ? shipTile : horseTile);

                spriteRenderer.sprite = normalSprite;  // Change player sprite to normal

                // Changing the indications
                allowedTiles.isOnShip = false;
                allowedTiles.isOnHorse = false;

                // You could use that axe only when not on a vehicle.
                if(allowedTiles.hasPickaxe)
                {
                    usingPicaxe = true;
                }
            }
        }
        else if (tileOnNewPosition == shipTile)
        {
            // Step onto a ship tile: become a ship and replace the tile with shallow sea

            usingPicaxe = false; // You cannot use the pickaxe while on the ship.
            allowedTiles.isOnShip = true;
            spriteRenderer.sprite = shipSprite;  // Change sprite to ship

            Vector3Int newPos = tilemap.WorldToCell(newPosition);
            SetTileAtPosition(newPos, shallowSeaTile);  // Replace with shallow sea tile
        }
        else if (tileOnNewPosition == horseTile)
        {
            // Step onto a horse tile: become a horse and replace the tile with appropriate terrain

            usingPicaxe = false; // You cannot use the pickaxe while on the horse.
            allowedTiles.isOnHorse = true;
            spriteRenderer.sprite = horseSprite;

            Vector3Int newPos = tilemap.WorldToCell(newPosition);
            SetTileAtPosition(newPos, mountainTile);  // Replace with horse terrain
        }
    }

    // Handle pickaxe interaction and destruction of mountain tiles
    private void HandlePickaxe(Vector3 newPosition, TileBase tileOnNewPosition)
    {
        if (tileOnNewPosition == pickaxeTile && !allowedTiles.hasPickaxe)
        {
            // Player picks up the pickaxe, and could use it immediately.
            allowedTiles.hasPickaxe = true;
            usingPicaxe = true;

            Vector3Int pickaxePosition = tilemap.WorldToCell(newPosition); 
            tilemap.SetTile(pickaxePosition, grassTile); // Replace pickaxe tile with grass
        }

        // If the player has a pickaxe and steps on a mountain tile, destroy it and replace with grass
        if (allowedTiles.hasPickaxe && tileOnNewPosition == mountainTile && usingPicaxe)
        {
            // Set the tile to grass where the mountain tile is
            Vector3Int cellPosition = tilemap.WorldToCell(newPosition); 
            tilemap.SetTile(cellPosition, grassTile);
        }
    }

    private void SetTileAtPosition(Vector3Int position, TileBase tile)
    {
        tilemap.SetTile(position, tile);  // Set the tile at the given position
    }
}