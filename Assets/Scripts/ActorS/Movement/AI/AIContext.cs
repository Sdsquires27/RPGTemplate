// Assets/Scripts/AI/AIContext.cs
public class AIContext
{
    public AIScript self;
    public ActorScript target;
    public HexGrid hexGrid;
    public HexTile currentTile;
    public float healthPercent;
    public float detectionRange;
    public int attackDamage;
}