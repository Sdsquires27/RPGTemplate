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
        // TODO: Score each goal based on current conditions
        // Examples of what to consider:
        // - Is a target detected and within range?
        // - Is an ally nearby and low health?
        // - Has the strategic directive changed?
        // - Has this AI taken damage recently?
        // - Has the AI heard a sound or seen movement?
        // After scoring, set currentGoal to the winner
    }

    public float GetGoalMultiplier(string actionName)
    {
        // TODO: Return a multiplier per action based on current goal
        // This is what biases the Utility AI scoring
        // Example structure:
        // if currentGoal == HuntTarget:
        //     Chase -> 1.5x, Attack -> 1.5x, Flee -> 0.5x, Wander -> 0.1x
        // if currentGoal == Retreat:
        //     Flee -> 2.0x, Chase -> 0.0x, Attack -> 0.2x, Wander -> 0.5x
        return 1f; // neutral multiplier until implemented
    }

    public void ReceiveDirective(StrategicDirective directive)
    {
        // TODO: Map strategic directives to goal biases
        // e.g. Assault directive → heavily bias toward HuntTarget
        // e.g. Defend directive → heavily bias toward ProtectAlly
    }
}