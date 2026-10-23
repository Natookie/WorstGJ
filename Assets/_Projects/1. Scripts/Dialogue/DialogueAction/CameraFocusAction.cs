using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Camera Focus World")]
public class CameraFocusAction : DialogueAction
{
    [SerializeField] private string targetId;

    public override void Execute(){
        var target = TransformRegistry.Instance?.Get(targetId);
        if(target == null){
            Debug.LogWarning($"[CameraFocusAction] No world target with id '{targetId}'");
            return;
        }

        CameraController.Instance?.FocusOnWorldTarget(target);
    }
}