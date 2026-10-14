using System;
using UnityEngine;

public static class SaveSystem
{
    public const int DefaultSlot = 0;

    public static event Action<int> OnSaved;
    public static event Action<int> OnLoaded;

    public static bool Save(int slot = DefaultSlot){
        try{
            var data = GameDataStore.Current;
            data.lastSavedUtc = DateTime.UtcNow.ToString("o");
            SaveFileHandler.Write(data, slot);
            OnSaved?.Invoke(slot);
            return true;
        }
        catch (Exception e){
            Debug.LogError($"[SaveSystem] Save failed: {e}");
            return false;
        }
    }

    public static GameData Load(int slot = DefaultSlot){
        var data = SaveFileHandler.Read(slot);
        if (data == null) return null;

        GameDataStore.Set(data);
        ApplyToScene(data);
        OnLoaded?.Invoke(slot);
        return data;
    }

    public static void Delete(int slot = DefaultSlot) => SaveFileHandler.Delete(slot);
    public static bool Exists(int slot = DefaultSlot) => SaveFileHandler.Exists(slot);

    private static void ApplyToScene(GameData data) { }
}