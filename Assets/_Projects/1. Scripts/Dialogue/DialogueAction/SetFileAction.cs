using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Set Flag")]
public class SetFlagAction : DialogueAction
{
    [SerializeField] private string flagId;
    [SerializeField] private bool value = true;

    public override void Execute(){
        if(GameDataStore.Current == null){
            Debug.LogWarning("[SetFlagAction] GameDataStore.Current is null");
            return;
        }
        if(string.IsNullOrEmpty(flagId)){
            Debug.LogWarning("[SetFlagAction] flagId is empty");
            return;
        }
        GameDataStore.Current.SetFlag(flagId, value);
    }
}