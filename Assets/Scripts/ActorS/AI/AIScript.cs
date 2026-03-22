using UnityEngine;

public class AIScript : ActorScript
{
    [Header("AI Movement")]
    [SerializeField] private float moveCooldown = 1f;
    private float lastMoveTime;
    protected Blackboard blackboard;
    protected GoalLayer goalLayer;
    protected UtilityLayer utilityLayer;
    protected StrategicLayer strategicLayer;

    public bool IsMoving => isMoving;
    public HexTile CurrentTile => currentTile;
    public HexGrid HexGrid => hexGrid;
    public float HealthPercent => (float)health / maxHealth;

    protected override void Start()
    {
        base.Start();
        blackboard = new Blackboard();
        goalLayer = new GoalLayer(this, blackboard);
        utilityLayer = BuildUtilityLayer(); // virtual so subclasses can add their own actions
        strategicLayer = FindFirstObjectByType<StrategicLayer>();
        strategicLayer?.RegisterAgent(this);
    }

    // Subclasses override this to register their specific actions
    protected virtual UtilityLayer BuildUtilityLayer()
    {
        return new UtilityLayer(this, blackboard, goalLayer);
    }

    protected override void HandleMovement()
    {
        goalLayer.Tick();

        // Only tick utility layer if cooldown has passed
        if (Time.time - lastMoveTime < moveCooldown) return;
        utilityLayer.Tick(GetContext());
    }
    
    public override void MoveToTile(HexTile tile)
    {
        base.MoveToTile(tile);
        lastMoveTime = Time.time;
    }

    protected override void HandleActions() { }

    public void ReceiveDirective(StrategicDirective directive)
    {
        goalLayer.ReceiveDirective(directive);
    }

    public virtual AIContext GetContext()
    {
        return new AIContext
        {
            self = this,
            target = blackboard.Get<ActorScript>("target"),
            hexGrid = hexGrid,
            currentTile = currentTile,
            healthPercent = HealthPercent,
        };
    }

    new void OnDestroy()
    {
        strategicLayer?.UnregisterAgent(this);
        currentTile?.ClearActor();
    }
}