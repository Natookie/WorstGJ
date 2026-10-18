using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Nova;

public class WorldSpaceNovaPointer : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private UIBlock2D rtDisplay;
    [SerializeField] private uint pointerId = 1;

    private readonly List<UIBlockHit> hitsBuffer = new();

    void Update(){
        if(worldCamera == null || uiCamera == null || rtDisplay == null || Mouse.current == null)
            return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray uiRay = uiCamera.ScreenPointToRay(screenPos);

        hitsBuffer.Clear();
        Interaction.RaycastAll(uiRay, hitsBuffer);

        UIBlockHit hit = default;
        bool found = false;
        foreach(var h in hitsBuffer){
            if(h.UIBlock == rtDisplay){ 
                hit = h; 
                found = true; 
                break; 
            }
        }

        if(!found){
            Interaction.Point(new Interaction.Update(default, pointerId), false);
            return;
        }

        Vector3 local = rtDisplay.transform.InverseTransformPoint(hit.Position);
        Vector2 size = rtDisplay.CalculatedSize.XY.Value;
        Vector2 uv = new Vector2(local.x / size.x + 0.5f, local.y / size.y + 0.5f);

        Ray worldRay = worldCamera.ViewportPointToRay(uv);
        Interaction.Point(new Interaction.Update(worldRay, pointerId), Mouse.current.leftButton.isPressed);
    }
}