// Assets/Scripts/AI/AIScript.cs
using UnityEngine;

public class AIScript : ActorScript
{
    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask targetLayer;

    // The three layers
    private StrategicLayer strategicLayer;  // shared, found in scene
    private GoalLayer goalLayer;            // per agent
    private UtilityLayer utilityLayer;      // per agent

    private Blackboard blackboard;

    // Exposed properties for action/condition nodes
    public bool IsMoving => isMoving;
    public HexTile CurrentTile => currentTile;
    public HexGrid HexGrid => hexGrid;
    public float HealthPercent => (float)health / maxHealth;

    protected override void Start()
    {
        base.Start();

        blackboard = new Blackboard();
        goalLayer = new GoalLayer(this, blackboard);
        utilityLayer = new UtilityLayer(this, blackboard, goalLayer);

        // Find and register with the strategic layer if one exists
        strategicLayer = FindFirstObjectByType<StrategicLayer>();
        strategicLayer?.RegisterAgent(this);
    }

    void OnDestroy()
    {
        strategicLayer?.UnregisterAgent(this);
    }

    protected override void HandleMovement()
    {
        // Tick goal layer on its slower interval
        goalLayer.Tick();

        // Tick utility layer every frame
        utilityLayer.Tick(GetContext());
    }

    public void ReceiveDirective(StrategicDirective directive)
    {
        goalLayer.ReceiveDirective(directive);
    }

    AIContext GetContext()
    {
        return new AIContext
        {
            self = this,
            target = blackboard.Get<ActorScript>("target"),
            hexGrid = hexGrid,
            currentTile = currentTile,
            healthPercent = HealthPercent,
            detectionRange = detectionRange,
            attackDamage = attackDamage
        };
    }

    protected override void HandleActions() { }
}