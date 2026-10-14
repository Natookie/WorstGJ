using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetDoorName;
    [SerializeField] private string label = "Door";

    public string Id => gameObject.name;
    public string TargetId => targetDoorName;

    public bool CanInteract => true;
    public Transform Transform => transform;
    public string promptMessage => label;

    public void OnInteract(GameObject interactor) => RoomManager.Instance.Travel(this);
}