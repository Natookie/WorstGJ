using UnityEngine;
using UnityEngine.Events;
using Nova;
using NaughtyAttributes;

[RequireComponent(typeof(Nova.Interactable))]
public class CoreButton : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D containerBlock;
    [SerializeField] private UIBlock2D baseBlock;
    [SerializeField] private UIBlock2D baseIconBlock;
    [SerializeField] private TextBlock baseTextBlock;
    [SerializeField] private UIBlockAnimator animator;

    [Header("STATE")]
    [SerializeField] private bool animate = true;

    [Header("COLOR SYNC")]
    [SerializeField] private bool syncIconWithText = true;

    [Header("CHANNEL")]
    [SerializeField] private ButtonClickChannel clickChannel;
    [SerializeField] private string clickId;

    [Header("EVENTS")]
    [SerializeField] private UnityEvent onClick   = new UnityEvent();
    [SerializeField] private UnityEvent onHover   = new UnityEvent();
    [SerializeField] private UnityEvent onUnhover = new UnityEvent();

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
        public Vector2 from;
        public Vector2 to;
    }

    [System.Serializable]
    public struct MoveSet
    {
        public Vector2 from;
        public Vector2 to;
    }

    [System.Serializable]
    public struct ShakeSet
    {
        public float amplitude;
        public float frequency;
        public float duration;
    }

    [Header("COLORS")]
    [ShowIf("animate")] [SerializeField] private bool showColor = true;
    [ShowIf("ShowBlockColor")] [SerializeField] private ColorSet baseColors;
    [ShowIf("ShowTextColor")]  [SerializeField] private ColorSet textColors;
    [ShowIf("ShowIconColor")]  [SerializeField] private ColorSet iconColors;

    [Header("ANIMATION")]
    [ShowIf("animate")] [SerializeField] private float colorDuration = 0.1f;
    [ShowIf("animate")] [SerializeField] private float hoverDuration = 0.15f;
    [ShowIf("animate")] [SerializeField] private float clickDuration = 0.1f;

    [Header("HOVER ANIMATION")]
    [ShowIf("animate")] [SerializeField] private bool scaleOnHover = false;
    [ShowIf("animate")] [SerializeField] private bool moveOnHover = false;
    [ShowIf("animate")] [SerializeField] private bool shakeOnHover = false;

    [ShowIf("scaleOnHover")] [SerializeField] private ScaleSet hoverScale;
    [ShowIf("moveOnHover")]  [SerializeField] private MoveSet hoverMove;
    [ShowIf("shakeOnHover")] [SerializeField] private ShakeSet hoverShake;

    [Header("CLICK ANIMATION")]
    [ShowIf("animate")] [SerializeField] private bool scaleOnClick = false;
    [ShowIf("animate")] [SerializeField] private bool moveOnClick = false;
    [ShowIf("animate")] [SerializeField] private bool shakeOnClick = false;

    [ShowIf("scaleOnClick")] [SerializeField] private ScaleSet clickScale;
    [ShowIf("moveOnClick")]  [SerializeField] private MoveSet clickMove;
    [ShowIf("shakeOnClick")] [SerializeField] private ShakeSet clickShake;

    bool ShowBlockColor => animate && showColor && baseBlock != null;
    bool ShowTextColor  => animate && showColor && baseTextBlock != null;
    bool ShowIconColor  => animate && showColor && baseIconBlock != null;

    ColorSet EffectiveIconColors => syncIconWithText ? textColors : iconColors;

    [Button("Initialize", EButtonEnableMode.Editor)]
    public void Init(){
        containerBlock = GetComponent<UIBlock2D>();
        if(containerBlock) baseBlock = containerBlock.GetChild(0).GetComponent<UIBlock2D>();
        if(baseBlock) baseTextBlock = baseBlock.GetChild(1).GetComponent<TextBlock>();
        if(baseBlock) baseIconBlock = baseBlock.GetChild(0).GetComponent<UIBlock2D>();

        if(animator == null) animator = GetComponent<UIBlockAnimator>();
    }

    [Button("Add Animator", EButtonEnableMode.Editor)]
    public void AddAnimator(){
        if(animator == null) animator = gameObject.AddComponent<UIBlockAnimator>();
        animator.Assign(baseBlock, baseTextBlock, baseIconBlock);
    }

    void Awake(){
        if(containerBlock == null) Init();

        if(animator == null) animator = GetComponent<UIBlockAnimator>();
        if(animator == null) animate = false;
        if(animate) animator.CaptureOriginals();

        containerBlock.AddGestureHandler<Gesture.OnClick>(OnButtonClick);
        containerBlock.AddGestureHandler<Gesture.OnHover>(OnButtonHover);
        containerBlock.AddGestureHandler<Gesture.OnUnhover>(OnButtonUnhover);
    }

    void OnButtonClick(Gesture.OnClick evt){
        if(animate){
            if(ShowBlockColor) animator.AnimateBlockColor(baseColors.clicked, colorDuration);
            if(ShowTextColor)  animator.AnimateTextColor(textColors.clicked, colorDuration);
            if(ShowIconColor)  animator.AnimateIconColor(EffectiveIconColors.clicked, colorDuration);

            if(scaleOnClick) animator.AnimateScale(clickScale.from, clickScale.to, clickDuration);
            if(moveOnClick)  animator.AnimateMove(clickMove.from, clickMove.to, clickDuration);
            if(shakeOnClick) animator.AnimateShake(clickShake.amplitude, clickShake.frequency, clickShake.duration);
        }

        onClick?.Invoke();
        if(clickChannel != null) clickChannel.Raise(clickId);
    }

    void OnButtonHover(Gesture.OnHover evt){
        if(animate){
            if(ShowBlockColor) animator.AnimateBlockColor(baseColors.hover, colorDuration);
            if(ShowTextColor)  animator.AnimateTextColor(textColors.hover, colorDuration);
            if(ShowIconColor)  animator.AnimateIconColor(EffectiveIconColors.hover, colorDuration);

            if(scaleOnHover) animator.AnimateScale(hoverScale.from, hoverScale.to, hoverDuration);
            if(moveOnHover)  animator.AnimateMove(hoverMove.from, hoverMove.to, hoverDuration);
            if(shakeOnHover) animator.AnimateShake(hoverShake.amplitude, hoverShake.frequency, hoverShake.duration);
        }

        onHover?.Invoke();
    }

    void OnButtonUnhover(Gesture.OnUnhover evt){
        if(animate){
            animator.ResetToOriginals(colorDuration);

            if(scaleOnHover || scaleOnClick) animator.AnimateScale(Vector3.one, Vector3.one, hoverDuration, true);
            if(moveOnHover  || moveOnClick)  animator.AnimateMove(Vector3.zero, Vector3.zero, hoverDuration, true);
        }

        onUnhover?.Invoke();
    }
}