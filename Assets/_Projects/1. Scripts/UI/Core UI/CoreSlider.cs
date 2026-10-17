using UnityEngine;
using Nova;
using UnityEngine.Events;
using NaughtyAttributes;

[System.Serializable]
public class SliderValueChangedEvent : UnityEvent<float> { }

[RequireComponent(typeof(Nova.Interactable))]
public class CoreSlider : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D containerBlock;
    [SerializeField] private UIBlock2D background;
    [SerializeField] private UIBlock2D fill;
    [SerializeField] private UIBlock2D draggable;

    [Header("VALUE")]
    [Range(0f, 1f)] [SerializeField] private float defaultValue = 0.5f;
    [SerializeField] private float sizeOffset = 10f;

    [Header("EVENT")]
    [SerializeField] private SliderValueChangedEvent onValueChanged = new SliderValueChangedEvent();
    public SliderValueChangedEvent OnValueChanged => onValueChanged;

    private float normalizedValue;
    private float fillPercentage;
    private bool isDragging;

    public float NormalizedValue => normalizedValue;

    void Awake(){
        Init();
        SetNormalizedValue(defaultValue, notify: false);
    }

    [Button("Initialize", EButtonEnableMode.Editor)]
    public void Init(){
        if(containerBlock == null) containerBlock = GetComponent<UIBlock2D>();

        containerBlock.AddGestureHandler<Gesture.OnPress>(HandlePointerDown);
        containerBlock.AddGestureHandler<Gesture.OnRelease>(HandlePointerUp);
        containerBlock.AddGestureHandler<Gesture.OnDrag>(HandleDrag);

        if(draggable != null) draggable.Position.X.Value = -draggable.Size.X.Value + sizeOffset;
    }

    void HandlePointerDown(Gesture.OnPress evt) => isDragging = true;
    void HandlePointerUp(Gesture.OnRelease evt) => isDragging = false;

    void HandleDrag(Gesture.OnDrag evt){
        if(!isDragging) return;

        float backgroundWidth = background.CalculatedSize.X.Value;
        float deltaPercent = evt.DragDeltaLocalSpace.x / backgroundWidth;

        fillPercentage += deltaPercent;
        fillPercentage = Mathf.Clamp01(fillPercentage);
        normalizedValue = fillPercentage;

        UpdateFill();
        onValueChanged.Invoke(normalizedValue);
    }

    void UpdateFill(){
        Length3 fillSize = fill.Size;
        fillSize.X.Percent = fillPercentage;
        fill.Size = fillSize;
    }

    public void SetNormalizedValue(float value, bool notify = true){
        normalizedValue = Mathf.Clamp01(value);
        fillPercentage = normalizedValue;
        UpdateFill();

        if(notify) onValueChanged.Invoke(normalizedValue);
    }
}