using UnityEngine;
using System.Collections;
using Nova;

public class UIBlockAnimator : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D baseBlock;
    [SerializeField] private TextBlock baseTextBlock;
    [SerializeField] private UIBlock2D baseIconBlock;

    [Header("DEFAULTS")]
    [SerializeField] private float colorDuration = 0.1f;

    private AnimationHandle blockColorHandle;
    private AnimationHandle textColorHandle;
    private AnimationHandle iconColorHandle;
    private AnimationHandle scaleHandle;
    private AnimationHandle moveHandle;
    private AnimationHandle shakeHandle;

    private Vector2 originalSize;
    private Vector2 originalPosition;
    private Color32 originalBlockColor;
    private Color32 originalTextColor;
    private Color32 originalIconColor;

    public Vector2 OriginalSize     => originalSize;
    public Vector2 OriginalPosition => originalPosition;
    public Vector2 CurrentPosition => baseBlock != null ? baseBlock.Position.XY.Value : Vector2.zero;

    void Start() => CaptureOriginals();

    public void Assign(UIBlock2D block, TextBlock text, UIBlock2D icon = null){
        baseBlock = block;
        baseTextBlock = text;
        baseIconBlock = icon;
        CaptureOriginals();
    }

    public void CaptureOriginals(){
        if(baseBlock != null){
            originalSize = baseBlock.Size.XY.Value;
            originalPosition = baseBlock.Position.XY.Value;
            originalBlockColor = baseBlock.Color;
        }
        if(baseTextBlock != null)
            originalTextColor = baseTextBlock.Color;
        if(baseIconBlock != null)
            originalIconColor = baseIconBlock.Color;
    }

    public void AnimateBlockColor(Color32 target, float duration){
        if(baseBlock == null) return;
        blockColorHandle.Cancel();
        if(baseBlock.Color.Equals(target)) return;
        blockColorHandle = new BlockColorAnimation{
            Block = baseBlock, From = baseBlock.Color, To = target
        }.Run(duration);
    }

    public void AnimateTextColor(Color32 target, float duration){
        if(baseTextBlock == null) return;
        textColorHandle.Cancel();
        if(baseTextBlock.Color.Equals(target)) return;
        textColorHandle = new TextColorAnimation{
            Text = baseTextBlock, From = baseTextBlock.Color, To = target
        }.Run(duration);
    }

    public void AnimateIconColor(Color32 target, float duration){
        if(baseIconBlock == null) return;
        iconColorHandle.Cancel();
        if(baseIconBlock.Color.Equals(target)) return;
        iconColorHandle = new BlockColorAnimation{
            Block = baseIconBlock, From = baseIconBlock.Color, To = target
        }.Run(duration);
    }

    public void AnimateScale(Vector2 fromMultiplier, Vector2 toMultiplier, float duration, bool fromCurrent = false){
        scaleHandle.Cancel();
        scaleHandle = new ScaleAnimation{
            Block = baseBlock,
            From = fromCurrent ? baseBlock.Size.XY.Value : Vector2.Scale(originalSize, fromMultiplier),
            To = Vector2.Scale(originalSize, toMultiplier)
        }.Run(duration);
    }

    public void AnimateMove(Vector2 fromTarget, Vector2 toTarget, float duration, bool fromCurrent = false, System.Action onComplete = null){
        moveHandle.Cancel();
        moveHandle = new MoveAnimation{
            Block = baseBlock,
            From = fromCurrent ? baseBlock.Position.XY.Value : fromTarget,
            To = toTarget
        }.Run(duration);

        if(onComplete != null) StartCoroutine(InvokeAfter(duration, onComplete));
    }

    public void AnimateShake(float amplitude, float frequency, float duration){
        if(Mathf.Approximately(amplitude, 0f) || duration <= 0f) return;
        shakeHandle.Cancel();
        shakeHandle = new ShakeAnimation{
            Block = baseBlock,
            Origin = originalPosition,
            Amplitude = amplitude,
            Frequency = frequency,
            Duration = duration
        }.Run(duration);
    }

    public void ResetToOriginals(float duration){
        if(baseBlock != null)     AnimateBlockColor(originalBlockColor, duration);
        if(baseTextBlock != null) AnimateTextColor(originalTextColor, duration);
        if(baseIconBlock != null) AnimateIconColor(originalIconColor, duration);

        AnimateScale(Vector2.one, Vector2.one, duration, true);
        AnimateMove(originalPosition, originalPosition, duration, true);
    }

    IEnumerator InvokeAfter(float seconds, System.Action cb){
        yield return new WaitForSeconds(seconds);
        cb?.Invoke();
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
    public UIBlock2D Block;
    public Vector2 From;
    public Vector2 To;

    public void Update(float percentDone){
        Block.Size.X.Value = Mathf.Lerp(From.x, To.x, percentDone);
        Block.Size.Y.Value = Mathf.Lerp(From.y, To.y, percentDone);
    }
}

public struct MoveAnimation : IAnimation
{
    public UIBlock2D Block;
    public Vector2 From;
    public Vector2 To;

    public void Update(float percentDone){
        Block.Position.X.Value = Mathf.Lerp(From.x, To.x, percentDone);
        Block.Position.Y.Value = Mathf.Lerp(From.y, To.y, percentDone);
    }
}

public struct ShakeAnimation : IAnimation
{
    public UIBlock2D Block;
    public Vector2 Origin;
    public float Amplitude;
    public float Frequency;
    public float Duration;

    public void Update(float percentDone){
        float decay = 1f - percentDone;
        float phase = percentDone * Duration * Frequency * Mathf.PI * 2f;
        float offset = Mathf.Sin(phase) * Amplitude * decay;
        Block.Position.X.Value = Origin.x + offset;
        Block.Position.Y.Value = Origin.y;
    }
}