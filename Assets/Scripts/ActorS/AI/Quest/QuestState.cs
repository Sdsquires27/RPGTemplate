using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines a single quest state for an NPC personality.
/// Represents a specific game scenario with desired items and objectives.
/// </summary>
[System.Serializable]
public class QuestState
{
    [Header("Quest Identity")]
    [SerializeField] public string questName = "New Quest";
    [SerializeField] public bool isActive = false;
    [Tooltip("Unique key to identify this quest state in GameState")]
    [SerializeField] public string gameStateKey = "quest_state_key";
    [Tooltip("If set, this quest auto-activates when this GameState bool becomes true")]
    [SerializeField] public string activationKey = "";

    [Header("Desires")]
    [Tooltip("Items the NPC wants/is tracking")]
    [SerializeField] public ItemData[] desiredItems = new ItemData[0];

    [Header("Objectives")]
    [Tooltip("GameState keys that this NPC cares about. If satisfied, quest is complete.")]
    [SerializeField] public string[] objectiveKeys = new string[0];
    [Tooltip("Should ALL objectives be true, or ANY?")]
    [SerializeField] public bool requireAllObjectives = true;

    [Header("Priority")]
    [Tooltip("How urgently the NPC pursues this quest (0-1)")]
    [SerializeField] public float questPriority = 0.8f;

    [Header("Movement")]
    [Tooltip("Maximum tiles the NPC will wander from home during this quest")]
    [SerializeField] public int wanderRadius = 3;

    /// <summary>
    /// Check if all or any objectives are met, based on requireAllObjectives.
    /// </summary>
    public bool AreObjectivesMet()
    {
        if (objectiveKeys.Length == 0) return false;

        if (requireAllObjectives)
        {
            // All must be true
            foreach (string key in objectiveKeys)
            {
                if (!GameState.GetBool(key, false))
                    return false;
            }
            return true;
        }
        else
        {
            // Any can be true
            foreach (string key in objectiveKeys)
            {
                if (GameState.GetBool(key, false))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Check if a specific item is one of the desired items.
    /// </summary>
    public bool IsDesiredItem(ItemData item)
    {
        foreach (ItemData desired in desiredItems)
        {
            if (desired == item)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Get all desired items for this quest.
    /// </summary>
    public ItemData[] GetDesiredItems()
    {
        return desiredItems;
    }

    /// <summary>
    /// Get the wander radius for this quest.
    /// </summary>
    public int GetWanderRadius()
    {
        return wanderRadius;
    }

    /// <summary>
    /// Check if this quest's activation condition is met.
    /// </summary>
    public bool IsActivationMet()
    {
        if (string.IsNullOrEmpty(activationKey))
            return false;
        return GameState.GetBool(activationKey, false);
    }
}
