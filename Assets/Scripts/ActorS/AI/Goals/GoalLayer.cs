// Assets/Scripts/AI/Goals/GoalLayer.cs
using UnityEngine;
using System.Collections.Generic;

public class GoalLayer
{
    public AIGoal currentGoal { get; private set; } = AIGoal.Patrol;

    private AIScript actor;
    private Blackboard blackboard;

    [SerializeField] private float evaluationInterval = 3f;
    private float lastEvaluation;

    public GoalLayer(AIScript actor, Blackboard blackboard)
    {
        this.actor = actor;
        this.blackboard = blackboard;
    }

    public void Tick()
    {
        if (Time.time - lastEvaluation < evaluationInterval) return;
        lastEvaluation = Time.time;
        EvaluateGoal();
    }

    void EvaluateGoal()
    {
        float seekItemScore = ScoreSeekItem();
        float patrolScore = 0.3f; // low constant fallback

        if (seekItemScore > patrolScore)
            currentGoal = AIGoal.SeekItem;
        else
            currentGoal = AIGoal.Patrol;
    }

    float ScoreSeekItem()
    {
        // High score if there's an item on the blackboard
        return blackboard.Has("seekItem") ? 0.9f : 0f;
    }

    public float GetGoalMultiplier(string actionName)
    {
        switch (currentGoal)
        {
            case AIGoal.SeekItem:
                if (actionName == "SeekItem") return 2f;
                if (actionName == "Wander")   return 0f;
                return 1f;
            case AIGoal.Patrol:
                if (actionName == "Wander")   return 1f;
                return 0.5f;
            default:
                return 1f;
        }
    }

    public void ReceiveDirective(StrategicDirective directive)
    {
        // TODO: Map strategic directives to goal biases
        // e.g. Assault directive → heavily bias toward HuntTarget
        // e.g. Defend directive → heavily bias toward ProtectAlly
    }
}