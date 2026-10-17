using UnityEngine;
using NaughtyAttributes;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawn;
    [SerializeField] private BoxCollider2D bounds;
    [SerializeField] private SpriteRenderer floorRenderer;

    [Space(10)]
    [Header("DESCRIPTION")]
    [TextArea(3, 20)][SerializeField] private string roomDescription;

    public Transform DefaultSpawn => defaultSpawn;
    public Collider2D Bounds => bounds;

    public void SetActive(bool value) => gameObject.SetActive(value);

    [Button("Initialize")]
    void Awake(){
        if(bounds == null) bounds = GetComponentInChildren<BoxCollider2D>();
        if(floorRenderer == null) floorRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if(defaultSpawn == null && bounds != null){
            var go = new GameObject("DefaultSpawn");
            go.transform.SetParent(transform);
            go.transform.position = bounds.bounds.center;
            defaultSpawn = go.transform;
        }
    }

    [Button("Quick Fix", EButtonEnableMode.Editor)]
    void QuickFix(){
        //if(floorRenderer != null) floorRenderer.drawMode = SpriteDrawMode.Tiled;
        Vector2 floorSize = floorRenderer.size;
        if(bounds != null){
            bounds.size = new Vector2(floorSize.x, floorSize.y);
            bounds.isTrigger = true;
            bounds.offset = new Vector2(floorSize.x, floorSize.y) * .5f;
        }
    }
}