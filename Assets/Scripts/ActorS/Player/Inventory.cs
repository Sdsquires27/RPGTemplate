using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 1; // start with 1 for simplicity
    private List<Item> heldItems = new List<Item>();

    public bool IsFull => heldItems.Count >= capacity;
    public bool IsEmpty => heldItems.Count == 0;

    public bool PickUp(Item item)
    {
        if (IsFull) return false;
        heldItems.Add(item);
        item.PickUp(transform);
        return true;
    }

    public bool Drop(HexTile tile)
    {
        if (IsEmpty || tile == null || !tile.isWalkable) return false;
        Item item = heldItems[0];
        heldItems.RemoveAt(0);
        item.transform.SetParent(null);
        item.Place(tile);
        return true;
    }

    public Item GetFirstItem() => IsEmpty ? null : heldItems[0];
}