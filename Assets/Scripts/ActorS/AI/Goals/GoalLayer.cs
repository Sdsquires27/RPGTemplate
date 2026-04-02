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
        float trackItemScore = ScoreTrackItem();
        float monitorObjectiveScore = ScoreMonitorObjective();
        float patrolScore = 0.3f; // low constant fallback

        // Personality-driven quest goals take priority
        float maxScore = Mathf.Max(trackItemScore, monitorObjectiveScore, patrolScore);

        if (maxScore == trackItemScore && trackItemScore > patrolScore)
            currentGoal = AIGoal.SeekItem; // Using SeekItem for item tracking
        else if (maxScore == monitorObjectiveScore && monitorObjectiveScore > patrolScore)
            currentGoal = AIGoal.Investigate; // Using Investigate for objective monitoring
        else
            currentGoal = AIGoal.Patrol;
    }

    float ScoreTrackItem()
    {
        // High score if there are tracked items the NPC is monitoring
        return blackboard.Has("trackedItems") ? 0.9f : 0f;
    }

    float ScoreMonitorObjective()
    {
        // Monitor objectives actively, even if not yet complete
        if (blackboard.Has("currentQuestPriority"))
        {
            float questPriority = blackboard.Get<float>("currentQuestPriority");
            return questPriority * 0.8f; // Slightly lower than item tracking
        }
        return 0f;
    }

    public float GetGoalMultiplier(string actionName)
    {
        switch (currentGoal)
        {
            case AIGoal.SeekItem:
                if (actionName == "TrackItem") return 2f;
                if (actionName == "Wander")   return 0f;
                return 1f;
            case AIGoal.Investigate:
                if (actionName == "MonitorObjective") return 2f;
                if (actionName == "Wander")          return 0.3f;
                return 0.8f;
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