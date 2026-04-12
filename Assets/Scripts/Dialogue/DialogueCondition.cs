using UnityEngine;

public enum ConditionType
{
    BoolEquals,
    IntEquals,
    IntGreaterThan,
    IntLessThan,
    IntGreaterThanOrEqual,
    IntLessThanOrEqual,
    HoldsItem
}

[System.Serializable]
public class DialogueCondition
{
    public string variableName;
    public ConditionType type;
    public int intValue;
    public bool boolValue;
    public ItemData requiredItem;

    public bool Evaluate()
    {
        switch (type)
        {
            case ConditionType.BoolEquals:
                return GameState.GetBool(variableName) == boolValue;
            case ConditionType.IntEquals:
                return GameState.GetInt(variableName) == intValue;
            case ConditionType.IntGreaterThan:
                return GameState.GetInt(variableName) > intValue;
            case ConditionType.IntLessThan:
                return GameState.GetInt(variableName) < intValue;
            case ConditionType.IntGreaterThanOrEqual:
                return GameState.GetInt(variableName) >= intValue;
            case ConditionType.IntLessThanOrEqual:
                return GameState.GetInt(variableName) <= intValue;
            case ConditionType.HoldsItem:
                Inventory inv = GameServices.GetPlayerInventory();
                if (inv == null) return false;
                Item held = inv.GetFirstItem();
                return held != null && held.data == requiredItem;
            default:
                return false;
        }
    }
}

[System.Serializable]
public class DialogueStateChange
{
    public enum ChangeType { SetBool, SetInt, IncrementInt, DecrementInt, GiveItem, TakeItem  }

    public ChangeType type;
    public string variableName;
    public int intValue;
    public bool boolValue;
    public ItemData item;

    public void Apply()
    {
        switch (type)
        {
            case ChangeType.SetBool:
                GameState.SetBool(variableName, boolValue);
                break;
            case ChangeType.SetInt:
                GameState.SetInt(variableName, intValue);
                break;
            case ChangeType.IncrementInt:
                GameState.SetInt(variableName, GameState.GetInt(variableName) + intValue);
                break;
            case ChangeType.DecrementInt:
                GameState.SetInt(variableName, GameState.GetInt(variableName) - intValue);
                break;
            case ChangeType.GiveItem:
                GiveItemToPlayer();
                break;
            case ChangeType.TakeItem:
                TakeItemFromPlayer();
                break;        
        }
    }

private void TakeItemFromPlayer()
{
    if (item == null) { Debug.LogWarning("[TakeItem] item is null"); return; }

    Inventory inventory = GameServices.GetPlayerInventory();
    if (inventory == null) { Debug.LogWarning("[TakeItem] inventory is null"); return; }

    Item heldItem = inventory.GetFirstItem();
    if (heldItem == null) { Debug.LogWarning("[TakeItem] no item held"); return; }
    
    Debug.Log($"[TakeItem] held: {heldItem.data?.name}, expected: {item?.name}, match: {heldItem.data == item}");
    
    if (heldItem.data != item) 
    { 
        Debug.LogWarning("[TakeItem] player isn't holding the expected item"); 
        return; 
    }

    bool removed = inventory.Remove(heldItem);
    Debug.Log($"[TakeItem] removed from inventory: {removed}, destroying: {heldItem.gameObject.name}");
    Object.Destroy(heldItem.gameObject);
}

private void GiveItemToPlayer()
{
    if (item == null) { Debug.LogWarning("[GiveItem] item is null"); return; }

    Inventory inventory = GameServices.GetPlayerInventory();
    if (inventory == null) { Debug.LogWarning("[GiveItem] inventory is null - is GameServices set up?"); return; }
    if (inventory.IsFull) { Debug.LogWarning("[GiveItem] inventory is full"); return; }
    if (item.worldPrefab == null) { Debug.LogWarning("[GiveItem] worldPrefab is null"); return; }

    GameObject itemObject = Object.Instantiate(item.worldPrefab);
    SpriteRenderer sr = itemObject.GetComponent<SpriteRenderer>();
    if (sr != null)
        sr.sortingOrder = 50;
    Item spawnedItem = itemObject.GetComponent<Item>();
    if (spawnedItem == null)
        spawnedItem = itemObject.AddComponent<Item>();

    spawnedItem.data = item;
    inventory.PickUp(spawnedItem);
}
}