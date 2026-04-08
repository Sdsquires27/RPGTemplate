using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    public HexTile currentTile { get; private set; }

    void Start()
    {
        // Find the nearest tile and place on it
        HexGrid grid = FindFirstObjectByType<HexGrid>();
        HexTile tile = grid.GetNearestTile(transform.position);
        if (tile != null) Place(tile);
    }
    public void Place(HexTile tile)
    {
        // Clear old tile
        if (currentTile != null)
            currentTile.ClearOccupant();

        currentTile = tile;
        tile.SetOccupant(this);
        transform.position = tile.transform.position;
        gameObject.SetActive(true);
    }

    public void PickUp(Transform carriedBy)
    {
        if (currentTile != null)
        {
            currentTile.ClearOccupant();
            currentTile = null;
        }

        // Attach to carrier in world space
        transform.SetParent(carriedBy);
        transform.localPosition = new Vector3(0.2f, 0.2f, -0.1f); // slight offset so it's visible
        gameObject.SetActive(true);

        // Apply item-driven GameState changes for pickup
        data?.ApplyStateChanges(data.onPickedUp);
    }
}