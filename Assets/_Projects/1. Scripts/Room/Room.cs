using UnityEngine;
using NaughtyAttributes;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawn;
    [SerializeField] private Collider2D bounds;

    public Transform DefaultSpawn => defaultSpawn;
    public Collider2D Bounds => bounds;

    public void SetActive(bool value) => gameObject.SetActive(value);

    [Button("Initialize")]
    void Awake(){
        if(bounds == null) bounds = GetComponentInChildren<Collider2D>();
        if(defaultSpawn == null && bounds != null){
            var go = new GameObject("DefaultSpawn");
            go.transform.SetParent(transform);
            go.transform.position = bounds.bounds.center;
            defaultSpawn = go.transform;
        }
    }
}