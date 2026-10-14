using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveFileHandler
{
    private const string FileNameFormat = "save_{0}.json";

    public static string GetPath(int slot) => Path.Combine(Application.persistentDataPath, string.Format(FileNameFormat, slot));
    public static bool Exists(int slot) => File.Exists(GetPath(slot));

    public static void Write(GameData data, int slot){
        string path = GetPath(slot);
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(path, json, Encoding.UTF8);
        Debug.Log($"[SaveSystem] Saved slot {slot} → {path}");
    }

    public static GameData Read(int slot){
        string path = GetPath(slot);
        if (!File.Exists(path)) return null;

        try{
            string json = File.ReadAllText(path, Encoding.UTF8);
            return JsonUtility.FromJson<GameData>(json);
        }
        catch (Exception e){
            Debug.LogError($"[SaveSystem] Read failed: {e.Message}");
            return null;
        }
    }

    public static void Delete(int slot){
        string path = GetPath(slot);
        if(File.Exists(path)) File.Delete(path);
    }
}