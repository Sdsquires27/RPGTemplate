using UnityEngine;
using System.Collections.Generic;

public static class GameState
{
    private static Dictionary<string, bool> bools = new Dictionary<string, bool>();
    private static Dictionary<string, int> ints = new Dictionary<string, int>();

    // -------------------------------------------------------------------------
    // Booleans
    // -------------------------------------------------------------------------

    public static void SetBool(string key, bool value) => bools[key] = value;

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return bools.TryGetValue(key, out bool value) ? value : defaultValue;
    }

    public static bool HasBool(string key) => bools.ContainsKey(key);

    // -------------------------------------------------------------------------
    // Integers
    // -------------------------------------------------------------------------

    public static void SetInt(string key, int value) => ints[key] = value;

    public static int GetInt(string key, int defaultValue = 0)
    {
        return ints.TryGetValue(key, out int value) ? value : defaultValue;
    }

    public static bool HasInt(string key) => ints.ContainsKey(key);

    // -------------------------------------------------------------------------
    // Persistence
    // -------------------------------------------------------------------------

    public static void Save()
    {
        // Save booleans
        foreach (var kvp in bools)
            PlayerPrefs.SetInt("bool_" + kvp.Key, kvp.Value ? 1 : 0);

        // Save integers
        foreach (var kvp in ints)
            PlayerPrefs.SetInt("int_" + kvp.Key, kvp.Value);

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        // TODO: load keys from a known list or saved manifest
        // For now, values are set at runtime — persistence can be expanded later
    }

    public static void Clear()
    {
        bools.Clear();
        ints.Clear();
    }

    // -------------------------------------------------------------------------
    // Debugging
    // -------------------------------------------------------------------------

    public static void PrintDebug()
    {
        Debug.Log("=== GAMESTATE DEBUG ===");
        
        if (bools.Count == 0)
            Debug.Log("  [Booleans] (none)");
        else
        {
            Debug.Log("  [Booleans]");
            foreach (var kvp in bools)
                Debug.Log($"    {kvp.Key} = {kvp.Value}");
        }

        if (ints.Count == 0)
            Debug.Log("  [Integers] (none)");
        else
        {
            Debug.Log("  [Integers]");
            foreach (var kvp in ints)
                Debug.Log($"    {kvp.Key} = {kvp.Value}");
        }

        Debug.Log($"Total: {bools.Count} bools, {ints.Count} ints");
        Debug.Log("======================");
    }

    public static string GetDebugString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== GAMESTATE DEBUG ===");
        
        sb.AppendLine("[Booleans]");
        if (bools.Count == 0)
            sb.AppendLine("  (none)");
        else
            foreach (var kvp in bools)
                sb.AppendLine($"  {kvp.Key} = {kvp.Value}");

        sb.AppendLine("[Integers]");
        if (ints.Count == 0)
            sb.AppendLine("  (none)");
        else
            foreach (var kvp in ints)
                sb.AppendLine($"  {kvp.Key} = {kvp.Value}");

        sb.AppendLine($"Total: {bools.Count} bools, {ints.Count} ints");
        sb.AppendLine("======================");
        
        return sb.ToString();
    }
}