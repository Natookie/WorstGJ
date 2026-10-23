using System.Collections.Generic;
using UnityEngine;

public class TransformRegistry : MonoBehaviour
{
    public static TransformRegistry Instance { get; private set; }

    [System.Serializable]
    public struct Entry
    {
        public string id;
        public Transform target;
    }

    [SerializeField] private List<Entry> entries = new();

    private Dictionary<string, Transform> _byId;

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _byId = new Dictionary<string, Transform>();
        foreach(var e in entries){
            if(string.IsNullOrEmpty(e.id) || e.target == null) continue;
            if(!_byId.TryAdd(e.id, e.target))
                Debug.LogWarning($"[TransformRegistry] Duplicate id '{e.id}' ignored.", this);
        }
    }

    public Transform Get(string id){
        if(string.IsNullOrEmpty(id) || _byId == null) return null;
        return _byId.TryGetValue(id, out var t) ? t : null;
    }
}