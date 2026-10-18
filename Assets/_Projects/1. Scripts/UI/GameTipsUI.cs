using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using Nova;

public class GameTipsUI : MonoBehaviour
{
    [SerializeField] private UIBlockAnimator uiBlockAnimator;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float duration;
    [Space(10)]
    [SerializeField] private Vector2 timer;

    private float elapsedTime;
    private float popupTime;
    private Vector2 originalPosition;
    private bool isShowing;
    private bool initialized;

    void Start(){
        StartCoroutine(InitAfterLayout());
    }

    IEnumerator InitAfterLayout(){
        yield return null;
        originalPosition = uiBlockAnimator.OriginalPosition;
        initialized = true;
        popupTime = Random.Range(timer.x, timer.y);
    }

    void Update(){
        if(!initialized) return;
        if(isShowing) return;
        if(GameManager.Instance.State != GameManager.GameState.Playing) return;

        elapsedTime += Time.deltaTime;
        if(elapsedTime >= popupTime){
            ShowGameTips();
            popupTime = Random.Range(timer.x, timer.y);
            elapsedTime = 0f;
        }
    }

    [Button("Show Game Tips", EButtonEnableMode.Playmode)]
    public void ShowGameTips(){
        isShowing = true;
        uiBlockAnimator.AnimateMove(originalPosition, targetPosition, duration, true);
    }

    [Button("Hide Game Tips", EButtonEnableMode.Playmode)]
    public void HideGameTips(){
        isShowing = false;
        uiBlockAnimator.AnimateMove(targetPosition, originalPosition, duration, true);
    }
}