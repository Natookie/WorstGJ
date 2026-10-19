using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public bool CanInteract => true;
    public Transform Transform => transform;
    public string promptMessage => "Talk";

    public void OnHover(GameObject interactor){}

    public void OnUnhover(GameObject interactor){}

    public void OnInteract(GameObject interactor){NPCInteract();}

    public virtual void NPCInteract()
    {
        if (!DialogueManager.Instance) return;

        DialogueLibrary.Instance.StartCoroutine(DialogueLibrary.Instance.TalkWithNPC());
    }
}
