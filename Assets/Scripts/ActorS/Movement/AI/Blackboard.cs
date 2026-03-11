// Assets/Scripts/AI/BehaviourTree/Blackboard.cs
using System.Collections.Generic;

public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public void Set<T>(string key, T value) => data[key] = value;

    public T Get<T>(string key)
    {
        if (data.TryGetValue(key, out object val) && val is T typed)
            return typed;
        return default;
    }

    public bool Has(string key) => data.ContainsKey(key);
    public void Clear(string key) => data.Remove(key);
}