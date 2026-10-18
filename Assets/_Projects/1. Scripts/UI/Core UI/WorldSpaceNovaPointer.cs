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
    [SerializeField] private uint scrollId = 2;
    [SerializeField] private float scrollMultiplier = 1f;
    [SerializeField] private bool invertScrolling = true;

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
            if(h.UIBlock == rtDisplay){ hit = h; found = true; break; }
        }

        if(!found){
            Interaction.Point(new Interaction.Update(default, pointerId), false);
            return;
        }

        Vector3 local = rtDisplay.transform.InverseTransformPoint(hit.Position);
        Vector2 size = rtDisplay.CalculatedSize.XY.Value;
        Vector2 uv = new Vector2(local.x / size.x + 0.5f, local.y / size.y + 0.5f);

        Ray worldRay = worldCamera.ViewportPointToRay(uv);

        Vector2 scrollDelta = Mouse.current.scroll.ReadValue().normalized;
        if(invertScrolling) scrollDelta.y *= -1f;
        if(scrollDelta != Vector2.zero){
            var scrollUpdate = new Interaction.Update(worldRay, scrollId);
            Interaction.Scroll(scrollUpdate, scrollDelta * scrollMultiplier);
        }

        bool pressed = Mouse.current.leftButton.isPressed;
        Interaction.Point(new Interaction.Update(worldRay, pointerId), pressed);

        if(Mouse.current.leftButton.wasPressedThisFrame){
            hitsBuffer.Clear();
            Interaction.RaycastAll(worldRay, hitsBuffer);

            UIBlock topHit = null;
            foreach(var h in hitsBuffer){
                if(h.UIBlock == rtDisplay) continue;
                topHit = h.UIBlock;
                break;
            }

            TextFieldManager.OnGlobalPress(topHit);
        }
    }
}