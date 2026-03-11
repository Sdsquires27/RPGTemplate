// Assets/Scripts/AI/Strategic/StrategicLayer.cs
using UnityEngine;
using System.Collections.Generic;

// One instance per faction/group - not per individual AI
public class StrategicLayer : MonoBehaviour
{
    public StrategicDirective currentDirective { get; private set; }

    [SerializeField] private float evaluationInterval = 15f;
    private float lastEvaluation;

    // All AI agents under this strategic layer's command
    private List<AIScript> agents = new List<AIScript>();

    void Update()
    {
        if (Time.time - lastEvaluation < evaluationInterval) return;
        lastEvaluation = Time.time;
        EvaluateDirective();
    }

    void EvaluateDirective()
    {
        // TODO: Score each directive based on faction-wide conditions
        // Examples of what to consider:
        // - How many allies are alive vs enemies?
        // - Is an objective being contested?
        // - Has the player crossed into a trigger zone?
        // - Are allies low on health overall?
        // After scoring, set currentDirective to the winner
        // Then broadcast it to all agents
        BroadcastDirective(currentDirective);
    }

    void BroadcastDirective(StrategicDirective directive)
    {
        foreach (var agent in agents)
            agent.ReceiveDirective(directive);
    }

    public void RegisterAgent(AIScript agent) => agents.Add(agent);
    public void UnregisterAgent(AIScript agent) => agents.Remove(agent);
}