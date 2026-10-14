using UnityEngine;

public static class GameDataStore
{
    public static GameData Current { get; private set; } = new GameData();

    public static void Set(GameData data) => Current = data ?? new GameData();
    public static void Reset() => Current = new GameData();
}