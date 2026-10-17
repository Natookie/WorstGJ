using UnityEngine;
using System.Collections;

public class DialogueLibrary : MonoBehaviour
{
    public static DialogueLibrary Instance { get; private set; }
    private DialogueManager dm;

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start(){
        dm = DialogueManager.Instance;
    }

    public IEnumerator PlayIntroSequence(){
        dm.PlayDialogue(SpeakerType.Orang1, "Halo semuanya perkenalkan nama saya Thomas Galih");
        yield return dm.WaitForSpace();

        dm.PlayDialogue(SpeakerType.Orang2, "Selamat pagi pak, saya anak baru di sini");
        yield return dm.WaitForSpace();

        dm.PlayDialogue(SpeakerType.Orang1, "Nyohohohohoh");
        yield return dm.WaitForSpace();

        dm.HideDialogue();
    }
}