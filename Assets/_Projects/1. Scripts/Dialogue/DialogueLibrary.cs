using System.Collections;
using UnityEngine;

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

    public IEnumerator Play(DialogueAsset asset){
        if(asset == null || asset.Lines == null || asset.Lines.Count == 0) yield break;

        if(dm == null) dm = DialogueManager.Instance;
        if(dm == null){
            Debug.LogError("[DialogueLibrary] DialogueManager.Instance is null");
            yield break;
        }

        dm.StartDialogue();

        for(int i = 0; i < asset.Lines.Count; i++){
            var line = asset.Lines[i];

            dm.PlayDialogue(line.speaker, line.text);

            if(line.onShownAction != null){
                foreach(var action in line.onShownAction)
                    action?.Execute();
            }
            line.onShown?.Invoke();

            yield return dm.WaitForSpace();

            if(line.onAdvancedAction != null){
                foreach(var action in line.onAdvancedAction)
                    action?.Execute();
            }
            line.onAdvanced?.Invoke();
        }

        dm.EndDialogue();
    }
}