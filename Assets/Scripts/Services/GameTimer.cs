using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float cutsceneTriggerTime = 300f; // 5 minutes
    [SerializeField] private UnityEvent onCutsceneTriggered;
    
    private float elapsed = 0f;
    private bool triggered = false;

    void Update()
    {
        if (triggered) return;
        elapsed += Time.unscaledDeltaTime;
        if (elapsed >= cutsceneTriggerTime)
        {
            triggered = true;   
            onCutsceneTriggered.Invoke();
        }
    }
}