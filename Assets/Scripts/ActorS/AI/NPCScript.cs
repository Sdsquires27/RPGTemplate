using UnityEngine;
using UnityEngine.InputSystem;

public class NPCScript : AIScript
{
    [Header("Personality")]
    [SerializeField] private PersonalityData personality;
    
    [Header("NPC Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] public int wanderRadius = 3; // Tiles from starting position
    [SerializeField] public HexTile homeTile; // Optional: specific home tile

    public HexTile startingTile { get; private set; }

    private float lastQuestCheckTime;
    [SerializeField] private float questCheckInterval = 0.5f; // Check every 0.5 seconds

    protected override void Start()
    {
        base.Start();
        startingTile = currentTile; // Record starting position
    }

    protected override void Update()
    {
        base.Update();
        
        // Auto-check for quest activation conditions
        if (Time.time - lastQuestCheckTime > questCheckInterval)
        {
            UpdateActiveQuestFromGameState();
            lastQuestCheckTime = Time.time;
        }
    }

    protected override UtilityLayer BuildUtilityLayer()
    {
        UtilityLayer layer = new UtilityLayer(this, blackboard, goalLayer);
        
        // Core actions - personality-driven
        if (personality != null)
        {
            layer.AddAction(new TrackItemAction(this, blackboard, goalLayer, personality));
            layer.AddAction(new MonitorObjectiveAction(this, blackboard, goalLayer, personality));
        }
        
        // Fallback actions
        layer.AddAction(new WanderAction(this, blackboard, goalLayer));
        return layer;
    }

    public override AIContext GetContext()
    {
        AIContext ctx = base.GetContext();
        ctx.detectionRange = detectionRange;
        return ctx;
    }

    /// <summary>
    /// Allows runtime quest switching without code changes.
    /// </summary>
    public void SetActiveQuest(string questName)
    {
        if (personality != null)
            personality.SetActiveQuest(questName);
    }

    /// <summary>
    /// Get the current active personality.
    /// </summary>
    public PersonalityData GetPersonality()
    {
        return personality;
    }

    /// <summary>
    /// Auto-switches quest based on GameState activation keys.
    /// Called periodically so quest transitions happen automatically.
    /// </summary>
    private void UpdateActiveQuestFromGameState()
    {
        if (personality == null) return;

        // Check each quest to see if its activation condition is met
        foreach (QuestState quest in personality.questStates)
        {
            if (quest.IsActivationMet())
            {
                // Condition is met - make this quest active
                SetActiveQuest(quest.questName);
                return; // Only switch to one quest at a time (first match)
            }
        }
    }
}