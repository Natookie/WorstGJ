using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int version = 1;

    //Player
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    //Progress
    public int currency;
    public int score;
    public int levelIndex;
    public List<ItemEntry> inventory = new();
    public List<DialogueFlag> flags = new();

    //Settings
    public SettingsData settings = new();

    //Meta Data
    public string lastSavedUtc;
    public float playTimeSeconds;

    public bool GetFlag(string id){
        if(flags == null) return false;
        var entry = flags.Find(f => f.id == id);
        return entry != null && entry.value;
    }

    public void SetFlag(string id, bool value){
        if(flags == null) flags = new List<DialogueFlag>();
        var entry = flags.Find(f => f.id == id);
        if(entry == null) flags.Add(new DialogueFlag { id = id, value = value });
        else entry.value = value;
    }
}

[Serializable]
public class ItemEntry
{
    public string itemId;
    public int quantity;
}

[Serializable]
public class SettingsData
{
    public float masterVolume = 1f;
    public float sfxVolume = .6f;
    public float bgmVolume = 1f;
    public bool fullscreen = true;

    public SettingsData Clone() => (SettingsData)MemberwiseClone();
}

[Serializable]
public class DialogueFlag
{
    public string id;
    public bool value;
}