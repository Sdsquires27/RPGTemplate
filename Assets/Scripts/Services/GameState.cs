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
}