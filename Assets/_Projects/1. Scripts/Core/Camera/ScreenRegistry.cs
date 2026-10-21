using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ScreenNodeEntry
{
    public ScreenDefinition definition;
    public Transform nodeTarget;
}

public class ScreenRegistry : MonoBehaviour
{
    public static ScreenRegistry Instance {get; private set;}

    [SerializeField] private List<ScreenNodeEntry> entries = new();
    private Dictionary<ScreenDefinition, Transform> _byDefinition;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }

        _byDefinition = new Dictionary<ScreenDefinition, Transform>();
        foreach(var entry in entries){
            if(entry.definition == null || entry.nodeTarget == null) continue;
            if(!_byDefinition.TryAdd(entry.definition, entry.nodeTarget))
                Debug.LogWarning($"Duplicate screen definition '{entry.definition.Id}' ignored.", entry.definition);

            entry.nodeTarget.gameObject.SetActive(false);
        }
    }

    public Transform GetNode(ScreenDefinition definition){
        if(definition == null) return null;
        return _byDefinition != null && _byDefinition.TryGetValue(definition, out var t) ? t : null;
    }
}