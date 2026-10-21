using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private Room startingRoom;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private Rigidbody2D playerRb;

    private readonly Dictionary<string, Door> doors = new();
    [ReadOnly] public Room currentRoom;

    void Awake(){
        if(Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start(){
        foreach(var door in FindObjectsByType<Door>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            doors[door.Id] = door;

        foreach(var room in FindObjectsByType<Room>(FindObjectsInactive.Include, FindObjectsSortMode.None)){
            if(room != startingRoom) room.SetActive(false);
        }

        if(startingRoom != null) EnterRoom(startingRoom, startingRoom.DefaultSpawn.position);
    }

    public void Travel(Door target, Door.EntryDirection entryDirection){
        if(target == null) return;

        var targetRoom = target.GetComponentInParent<Room>(true);
        Vector2 offset = entryDirection switch{
            Door.EntryDirection.Left  => Vector2.left,
            Door.EntryDirection.Right => Vector2.right,
            Door.EntryDirection.Up    => Vector2.up,
            Door.EntryDirection.Down  => Vector2.down,
            _ => Vector2.zero
        };
        Vector3 spawnPos = target.transform.position + (Vector3)offset * 2f;
        EnterRoom(targetRoom, spawnPos);
    }

    void EnterRoom(Room room, Vector3 spawn){
        if(room == null) return;

        if(currentRoom != null && currentRoom != room) currentRoom.SetActive(false);
        room.SetActive(true);
        currentRoom = room;

        ApplyBounds(room);

        if(playerRb != null){
            playerRb.position = spawn;
            playerRb.linearVelocity = Vector2.zero;
        }
        transform.position = spawn;
    }

    void ApplyBounds(Room room){
        if(room.Bounds == null) return;

        if(confiner != null){
            confiner.BoundingShape2D = room.Bounds;
            confiner.InvalidateBoundingShapeCache();
        }

        BoxConfinement2D roomConfinement = BoxConfinement2D.Instance; 
        if(roomConfinement != null) roomConfinement.SetBounds(room.Bounds);
    }
}