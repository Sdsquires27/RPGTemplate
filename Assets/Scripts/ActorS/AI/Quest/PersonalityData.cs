using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines an NPC's personality and quest-driven behavior.
/// Encapsulates all desires, objectives, and quest states without requiring code changes.
/// </summary>
[CreateAssetMenu(menuName = "NPC/PersonalityData")]
public class PersonalityData : ScriptableObject
{
    [Header("Personality")]
    [SerializeField] public string personalityName = "New Personality";
    [TextArea(2, 4)]
    [SerializeField] public string description = "Describe this personality...";

    [Header("Quest States")]
    [SerializeField] public QuestState[] questStates = new QuestState[0];

    /// <summary>
    /// Get the currently active quest state, or null if none are active.
    /// </summary>
    public QuestState GetActiveQuest()
    {
        foreach (QuestState quest in questStates)
        {
            if (quest.isActive)
                return quest;
        }
        return null;
    }

    /// <summary>
    /// Set a quest as active by name. Deactivates all others.
    /// </summary>
    public void SetActiveQuest(string questName)
    {
        foreach (QuestState quest in questStates)
        {
            quest.isActive = (quest.questName == questName);
        }
    }

    /// <summary>
    /// Get all desired items from the currently active quest.
    /// </summary>
    public ItemData[] GetActiveDesiredItems()
    {
        QuestState active = GetActiveQuest();
        if (active != null)
            return active.GetDesiredItems();
        return new ItemData[0];
    }

    /// <summary>
    /// Check if all quest states are complete.
    /// </summary>
    public bool AreAllQuestsComplete()
    {
        foreach (QuestState quest in questStates)
        {
            if (!quest.AreObjectivesMet())
                return false;
        }
        return true;
    }

    /// <summary>
    /// Get the priority of the currently active quest.
    /// </summary>
    public float GetActiveQuestPriority()
    {
        QuestState active = GetActiveQuest();
        if (active == null) return 0f;
        return active.questPriority;
    }

    /// <summary>
    /// Get the wander radius of the currently active quest.
    /// </summary>
    public int GetActiveQuestWanderRadius()
    {
        QuestState active = GetActiveQuest();
        if (active == null) return 3; // Default fallback
        return active.GetWanderRadius();
    }
}
