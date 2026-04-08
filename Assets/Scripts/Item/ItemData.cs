using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Basic")]
    public string itemName;
    public Sprite icon;
    public GameObject worldPrefab; // the item as it appears on the ground

    [Header("State Changes")]
    public DialogueStateChange[] onPickedUp;
    public DialogueStateChange[] onDropped;

    public void ApplyStateChanges(DialogueStateChange[] changes)
    {
        if (changes == null) return;
        foreach (var change in changes)
            change?.Apply();
    }
}