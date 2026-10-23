using UnityEngine;
using System.Collections;
using Nova;

public class GameTipsUI : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlockAnimator uiBlockAnimator;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float duration;
    [Space(10)]
    [SerializeField] private Vector2 timer;

    [Header("UI")]
    [SerializeField] private TextBlock tipsText;

    private static readonly string[] GAME_TIPS =
    {
        "Press W to move, use your mouse to set the direction",
        "Press E to interact with objects",
        "Type 'grab' to grab objects",
        "Type 'throw' to throw grabbed object",
        "Play the game to play the game",
    };

    private Vector2 originalPosition;
    private bool initialized;
    private bool _hideRequested;
    private Coroutine loopRoutine;

    void Start() => StartCoroutine(InitAfterLayout());

    IEnumerator InitAfterLayout(){
        yield return null;
        originalPosition = uiBlockAnimator.OriginalPosition;
        initialized = true;

        loopRoutine = StartCoroutine(TipsLoop());
    }

    IEnumerator TipsLoop(){
        while(GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Playing)
            yield return null;

        for(int i = 0; i < GAME_TIPS.Length; i++) yield return ShowAndWait(GAME_TIPS[i]);

        while(true){
            yield return null;
            if(GameManager.Instance.State != GameManager.GameState.Playing){
                yield return null;
                continue;
            }

            float wait = Random.Range(timer.x, timer.y);
            yield return new WaitForSeconds(wait);

            while(GameManager.Instance.State != GameManager.GameState.Playing)
                yield return null;

            string tip = GAME_TIPS[Random.Range(0, GAME_TIPS.Length)];
            yield return ShowAndWait(tip);
        }
    }

    IEnumerator ShowAndWait(string tipText){
        if(tipsText != null) tipsText.Text = tipText;

        bool showDone = false;
        uiBlockAnimator.AnimateMove(originalPosition, targetPosition, duration, true, () => showDone = true);
        while(!showDone) yield return null;

        _hideRequested = false;
        while(!_hideRequested) yield return null;

        bool hideDone = false;
        uiBlockAnimator.AnimateMove(targetPosition, originalPosition, duration, true, () => hideDone = true);
        while(!hideDone) yield return null;
    }

    public void HideGameTips(){
        _hideRequested = true;
    }

    public void ShowGameTips(){
        if(!initialized) return;
        if(loopRoutine == null) loopRoutine = StartCoroutine(TipsLoop());
    }

    void OnDestroy(){
        if(loopRoutine != null) StopCoroutine(loopRoutine);
    }
}