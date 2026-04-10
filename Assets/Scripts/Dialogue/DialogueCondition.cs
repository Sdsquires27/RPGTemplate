using UnityEngine;

public enum ConditionType
{
    BoolEquals,
    IntEquals,
    IntGreaterThan,
    IntLessThan,
    IntGreaterThanOrEqual,
    IntLessThanOrEqual
}

[System.Serializable]
public class DialogueCondition
{
    public string variableName;
    public ConditionType type;
    public int intValue;
    public bool boolValue;

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
            default:
                return false;
        }
    }
}

[System.Serializable]
public class DialogueStateChange
{
    public enum ChangeType { SetBool, SetInt, IncrementInt, DecrementInt, GiveItem }

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
        }
    }

    private void GiveItemToPlayer()
    {
        if (item == null) return;

        Inventory inventory = GameServices.GetPlayerInventory();
        if (inventory == null) return;
        if (inventory.IsFull) return;
        if (item.worldPrefab == null) return;

        GameObject itemObject = Object.Instantiate(item.worldPrefab);
        Item spawnedItem = itemObject.GetComponent<Item>();
        if (spawnedItem == null)
            spawnedItem = itemObject.AddComponent<Item>();

        spawnedItem.data = item;
        inventory.PickUp(spawnedItem);
    }
}