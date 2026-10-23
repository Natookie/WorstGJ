using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Open Screen")]
public class OpenScreenAction : DialogueAction
{
    [SerializeField] private ScreenDefinition screen;

    public override void Execute(){
        var node = ScreenRegistry.Instance?.GetNode(screen);
        if(node != null) CameraController.Instance?.EnterScreenMode(node);
    }
}