using UnityEngine;
using UnityEngine.Events;
using Nova;
using NaughtyAttributes;

[RequireComponent(typeof(Nova.Interactable))]
public class CoreButton : MonoBehaviour
{
    [Header("REFERENCES")]
    [Tooltip("Interactable block")][SerializeField] private UIBlock2D containerBlock;
    [Tooltip("Stylized block")][SerializeField] private UIBlock2D baseBlock;
    [Tooltip("Stylized text")][SerializeField] private TextBlock baseTextBlock;

    [Header("EVENTS")]
    [SerializeField] private UnityEvent onClick     = new UnityEvent();
    [SerializeField] private UnityEvent onHover     = new UnityEvent();
    [SerializeField] private UnityEvent onUnhover   = new UnityEvent();

    public UnityEvent OnClick   => onClick;
    public UnityEvent OnHover   => onHover;
    public UnityEvent OnUnhover => onUnhover;

    [System.Serializable]
    public struct ColorSet
    {
        public Color32 unhover;
        public Color32 hover;
        public Color32 clicked;
    }

    [System.Serializable]
    public struct ScaleSet
    {
        public Vector3 from;
        [Tooltip("Target multiplier")] public Vector3 to;
    }

    [System.Serializable]
    public struct MoveSet
    {
        public Vector3 from;
        [Tooltip("Target offset")] public Vector3 to;
    }

    [System.Serializable]
    public struct ShakeSet
    {
        [Tooltip("Amplitude of shake in local units")] public float amplitude;
        [Tooltip("Full shake cycles per second")]      public float frequency;
        [Tooltip("Duration of one shake burst")]       public float duration;
    }

    [Header("COLORS")]
    [SerializeField] private bool showColor = true;
    [ShowIf("ShowBlockColor")] [SerializeField] private ColorSet baseColors;
    [ShowIf("ShowTextColor")]  [SerializeField] private ColorSet textColors;

    [Header("ANIMATION")]
    [SerializeField] private float colorDuration = 0.1f;
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private float clickDuration = 0.1f;

    [Header("HOVER ANIMATION")]
    [SerializeField] private bool scaleOnHover = false;
    [SerializeField] private bool moveOnHover = false;
    [SerializeField] private bool shakeOnHover = false;

    [ShowIf("scaleOnHover")]  [SerializeField] private ScaleSet hoverScale;
    [ShowIf("moveOnHover")]   [SerializeField] private MoveSet hoverMove;
    [ShowIf("shakeOnHover")]  [SerializeField] private ShakeSet hoverShake;

    [Header("CLICK ANIMATION")]
    [SerializeField] private bool scaleOnClick = false;
    [SerializeField] private bool moveOnClick = false;
    [SerializeField] private bool shakeOnClick = false;

    [ShowIf("scaleOnClick")]  [SerializeField] private ScaleSet clickScale;
    [ShowIf("moveOnClick")]   [SerializeField] private MoveSet clickMove;
    [ShowIf("shakeOnClick")]  [SerializeField] private ShakeSet clickShake;

    bool ShowBlockColor => showColor && baseBlock != null;
    bool ShowTextColor  => showColor && baseTextBlock != null;

    AnimationHandle blockColorHandle;
    AnimationHandle textColorHandle;
    AnimationHandle scaleHandle;
    AnimationHandle moveHandle;
    AnimationHandle shakeHandle;

    Vector3 originalScale;
    Vector3 originalPosition;
    Color32 originalBlockColor;
    Color32 originalTextColor;

    [Button("Initialize", EButtonEnableMode.Editor)]
    public void Init(){
        containerBlock = GetComponent<UIBlock2D>();
        baseBlock = containerBlock.GetChild(0).GetComponent<UIBlock2D>();
        baseTextBlock = baseBlock.GetChild(0).GetComponent<TextBlock>();

        originalScale = baseBlock.transform.localScale;
        originalPosition = baseBlock.transform.localPosition;
        hoverScale.from = Vector3.one;
        hoverMove.from  = Vector3.zero;
        clickScale.from = Vector3.one;
        clickMove.from  = Vector3.zero;
    }

    [Button("Snapshot From", EButtonEnableMode.Editor)]
    public void SnapshotFrom(){
        hoverScale.from = Vector3.one;
        hoverMove.from  = Vector3.zero;
        clickScale.from = baseBlock.transform.localScale / originalScale.x;
        clickMove.from  = baseBlock.transform.localPosition - originalPosition;
    }

    void Awake(){
        if(containerBlock == null) Init();

        originalScale = baseBlock.transform.localScale;
        originalPosition = baseBlock.transform.localPosition;
        originalBlockColor = baseBlock.Color;
        originalTextColor = baseTextBlock.Color;

        containerBlock.AddGestureHandler<Gesture.OnClick>(OnButtonClick);
        containerBlock.AddGestureHandler<Gesture.OnHover>(OnButtonHover);
        containerBlock.AddGestureHandler<Gesture.OnUnhover>(OnButtonUnhover);
    }

    void OnButtonClick(Gesture.OnClick evt){
        if(ShowBlockColor) blockColorHandle = AnimateBlock(baseColors.clicked).Run(colorDuration);
        if(ShowTextColor)  textColorHandle  = AnimateText(textColors.clicked).Run(colorDuration);

        if(scaleOnClick) scaleHandle = AnimateScale(clickScale.from, clickScale.to).Run(clickDuration);
        if(moveOnClick)  moveHandle  = AnimateMove(clickMove.from, clickMove.to).Run(clickDuration);
        if(shakeOnClick) shakeHandle = MakeShake(clickShake);

        onClick?.Invoke();
    }

    void OnButtonHover(Gesture.OnHover evt){
        if(ShowBlockColor) blockColorHandle = AnimateBlock(baseColors.hover).Run(colorDuration);
        if(ShowTextColor)  textColorHandle  = AnimateText(textColors.hover).Run(colorDuration);

        if(scaleOnHover) scaleHandle = AnimateScale(hoverScale.from, hoverScale.to).Run(hoverDuration);
        if(moveOnHover)  moveHandle  = AnimateMove(hoverMove.from, hoverMove.to).Run(hoverDuration);
        if(shakeOnHover) shakeHandle = MakeShake(hoverShake);

        onHover?.Invoke();
    }

    void OnButtonUnhover(Gesture.OnUnhover evt){
        if(ShowBlockColor) blockColorHandle = AnimateBlock(originalBlockColor).Run(colorDuration);
        if(ShowTextColor)  textColorHandle  = AnimateText(originalTextColor).Run(colorDuration);

        if(scaleOnHover || scaleOnClick) scaleHandle = AnimateScale(Vector3.one, Vector3.one, true).Run(hoverDuration);
        if(moveOnHover  || moveOnClick)  moveHandle  = AnimateMove(Vector3.zero, Vector3.zero, true).Run(hoverDuration);

        onUnhover?.Invoke();
    }

    BlockColorAnimation AnimateBlock(Color32 target){
        blockColorHandle.Cancel();
        return new BlockColorAnimation{ Block = baseBlock, From = baseBlock.Color, To = target };
    }

    TextColorAnimation AnimateText(Color32 target){
        textColorHandle.Cancel();
        return new TextColorAnimation{ Text = baseTextBlock, From = baseTextBlock.Color, To = target };
    }

    ScaleAnimation AnimateScale(Vector3 fromMultiplier, Vector3 toMultiplier, bool fromCurrent = false){
        scaleHandle.Cancel();
        return new ScaleAnimation{
            Transform = baseBlock.transform,
            From = fromCurrent ? baseBlock.transform.localScale : Vector3.Scale(originalScale, fromMultiplier),
            To = Vector3.Scale(originalScale, toMultiplier)
        };
    }

    MoveAnimation AnimateMove(Vector3 fromOffset, Vector3 toOffset, bool fromCurrent = false){
        moveHandle.Cancel();
        return new MoveAnimation{
            Transform = baseBlock.transform,
            From = fromCurrent ? baseBlock.transform.localPosition : originalPosition + fromOffset,
            To = originalPosition + toOffset
        };
    }

    AnimationHandle MakeShake(ShakeSet set){
        if(Mathf.Approximately(set.amplitude, 0f) || set.duration <= 0f) return default;
        shakeHandle.Cancel();
        return new ShakeAnimation{
            Transform = baseBlock.transform,
            Origin = originalPosition,
            Amplitude = set.amplitude,
            Frequency = set.frequency,
            Duration = set.duration
        }.Run(set.duration);
    }
}

public struct BlockColorAnimation : IAnimation
{
    public UIBlock2D Block;
    public Color32 From;
    public Color32 To;
    public void Update(float percentDone) => Block.Color = Color32.Lerp(From, To, percentDone);
}

public struct TextColorAnimation : IAnimation
{
    public TextBlock Text;
    public Color32 From;
    public Color32 To;
    public void Update(float percentDone) => Text.Color = Color32.Lerp(From, To, percentDone);
}

public struct ScaleAnimation : IAnimation
{
    public Transform Transform;
    public Vector3 From;
    public Vector3 To;
    public void Update(float percentDone) => Transform.localScale = Vector3.Lerp(From, To, percentDone);
}

public struct MoveAnimation : IAnimation
{
    public Transform Transform;
    public Vector3 From;
    public Vector3 To;
    public void Update(float percentDone) => Transform.localPosition = Vector3.Lerp(From, To, percentDone);
}

public struct ShakeAnimation : IAnimation
{
    public Transform Transform;
    public Vector3 Origin;
    public float Amplitude;
    public float Frequency;
    public float Duration;

    public void Update(float percentDone){
        float decay = 1f - percentDone;
        float phase = percentDone * Duration * Frequency * Mathf.PI * 2f;
        float offset = Mathf.Sin(phase) * Amplitude * decay;
        Transform.localPosition = Origin + new Vector3(offset, 0f, 0f);
    }
}