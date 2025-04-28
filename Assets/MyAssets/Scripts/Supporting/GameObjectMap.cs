using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameObjectMap
{
    [SerializeField]
    private List<MapEntry> entries = new List<MapEntry>(); // List of serialized entries

    // Convert the serialized list into a usable dictionary
    public Dictionary<string, GameObject> ToDictionary()
    {
        Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
        foreach (var entry in entries)
        {
            if (!dictionary.ContainsKey(entry.key))
            {
                dictionary.Add(entry.key, entry.value);
            }
        }
        return dictionary;
    }

    // Add a new entry to the serialized list
    public void AddEntry(string key, GameObject value)
    {
        entries.Add(new MapEntry(key, value));
    }

    // Clear all entries
    public void Clear()
    {
        entries.Clear();
    }
}

[Serializable]
public class MapEntry
{
    public string key; // Key for the map entry
    public GameObject value; // Value for the map entry (e.g., a GameObject)

    public MapEntry(string key, GameObject value)
    {
        this.key = key;
        this.value = value;
    }
}