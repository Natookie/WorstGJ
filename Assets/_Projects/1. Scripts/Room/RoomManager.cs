using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private Room startingRoom;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private BoxConfinement2D playerConfinement;
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

        if(startingRoom != null) EnterRoom(startingRoom, startingRoom.DefaultSpawn);
    }

    public void Travel(Door from){
        if(from == null) return;

        if(!doors.TryGetValue(from.TargetId, out var target)){
            Debug.LogWarning($"Door '{from.Id}' targets '{from.TargetId}', which doesn't exist");
            return;
        }

        var targetRoom = target.GetComponentInParent<Room>(true);
        EnterRoom(targetRoom, target.transform);
    }

    void EnterRoom(Room room, Transform spawn){
        if(room == null) return;

        if(currentRoom != null && currentRoom != room) currentRoom.SetActive(false);
        room.SetActive(true);
        currentRoom = room;

        ApplyBounds(room);

        Vector3 spawnPos = spawn != null ? spawn.position : transform.position;

        if(playerRb != null){
            playerRb.position = spawnPos;
            playerRb.linearVelocity = Vector2.zero;
        }
        transform.position = spawnPos;
    }

    void ApplyBounds(Room room){
        if(room.Bounds == null) return;

        if(confiner != null){
            confiner.BoundingShape2D = room.Bounds;
            confiner.InvalidateBoundingShapeCache();
        }

        if(playerConfinement != null) playerConfinement.SetBounds(room.Bounds);
    }
}