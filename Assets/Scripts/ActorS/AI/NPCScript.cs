using UnityEngine;
using UnityEngine.InputSystem;

public class NPCScript : AIScript
{
    [Header("NPC Settings")]
    [SerializeField] private float detectionRange = 5f;

    protected override UtilityLayer BuildUtilityLayer()
    {
        UtilityLayer layer = new UtilityLayer(this, blackboard, goalLayer);
        layer.AddAction(new DetectItemAction(this, blackboard, goalLayer));
        layer.AddAction(new SeekItemAction(this, blackboard, goalLayer));
        layer.AddAction(new WanderAction(this, blackboard, goalLayer));
        return layer;
    }

    public override AIContext GetContext()
    {
        AIContext ctx = base.GetContext();
        ctx.detectionRange = detectionRange;
        return ctx;
    }
}