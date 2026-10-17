using UnityEngine;
using Nova;

public class UIBlockAnimator : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D baseBlock;
    [SerializeField] private TextBlock baseTextBlock;

    [Header("DEFAULTS")]
    [SerializeField] private float colorDuration = 0.1f;
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private float clickDuration = 0.1f;

    private AnimationHandle blockColorHandle;
    private AnimationHandle textColorHandle;
    private AnimationHandle scaleHandle;
    private AnimationHandle moveHandle;
    private AnimationHandle shakeHandle;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Color32 originalBlockColor;
    private Color32 originalTextColor;

    public Vector3 OriginalScale    => originalScale;
    public Vector3 OriginalPosition => originalPosition;

    void Awake() => CaptureOriginals();
    public void Assign(UIBlock2D block, TextBlock text){
        baseBlock = block;
        baseTextBlock = text;
    }

    public void CaptureOriginals(){
        if(baseBlock != null){
            originalScale = baseBlock.transform.localScale;
            originalPosition = baseBlock.transform.localPosition;
            originalBlockColor = baseBlock.Color;
        }
        if(baseTextBlock != null) originalTextColor = baseTextBlock.Color;
    }

    public void AnimateBlockColor(Color32 target, float duration){
        blockColorHandle.Cancel();
        blockColorHandle = new BlockColorAnimation{
            Block = baseBlock, From = baseBlock.Color, To = target
        }.Run(duration);
    }

    public void AnimateTextColor(Color32 target, float duration){
        textColorHandle.Cancel();
        textColorHandle = new TextColorAnimation{
            Text = baseTextBlock, From = baseTextBlock.Color, To = target
        }.Run(duration);
    }

    public void AnimateScale(Vector3 fromMultiplier, Vector3 toMultiplier, float duration, bool fromCurrent = false){
        scaleHandle.Cancel();
        scaleHandle = new ScaleAnimation{
            Transform = baseBlock.transform,
            From = fromCurrent ? baseBlock.transform.localScale : Vector3.Scale(originalScale, fromMultiplier),
            To = Vector3.Scale(originalScale, toMultiplier)
        }.Run(duration);
    }

    public void AnimateMove(Vector3 fromOffset, Vector3 toOffset, float duration, bool fromCurrent = false){
        moveHandle.Cancel();
        moveHandle = new MoveAnimation{
            Transform = baseBlock.transform,
            From = fromCurrent ? baseBlock.transform.localPosition : originalPosition + fromOffset,
            To = originalPosition + toOffset
        }.Run(duration);
    }

    public void AnimateShake(float amplitude, float frequency, float duration){
        if(Mathf.Approximately(amplitude, 0f) || duration <= 0f) return;
        shakeHandle.Cancel();
        shakeHandle = new ShakeAnimation{
            Transform = baseBlock.transform,
            Origin = originalPosition,
            Amplitude = amplitude,
            Frequency = frequency,
            Duration = duration
        }.Run(duration);
    }

    public void ResetToOriginals(float duration){
        AnimateBlockColor(originalBlockColor, duration);
        AnimateTextColor(originalTextColor, duration);
        AnimateScale(Vector3.one, Vector3.one, duration, true);
        AnimateMove(Vector3.zero, Vector3.zero, duration, true);
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