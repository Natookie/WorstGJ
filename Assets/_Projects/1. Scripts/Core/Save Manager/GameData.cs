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

    //Settings
    public SettingsData settings = new();

    //Meta Data
    public string lastSavedUtc;
    public float playTimeSeconds;
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