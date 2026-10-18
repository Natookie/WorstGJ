using UnityEngine;
using NaughtyAttributes;

public class GameTipsUI : MonoBehaviour
{
    [SerializeField] private UIBlockAnimator uiBlockAnimator;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float duration;
    [SerializeField] private Vector2 timer;
    [Space(10)]
    [ReadOnly] public float elapsedTime;
    [ReadOnly] public float popupTime;
    [ReadOnly] public Vector2 originalPosition;
    [ReadOnly] public bool isShowing;

    void Start(){
        originalPosition = uiBlockAnimator.OriginalPosition;
        popupTime = Random.Range(timer.x, timer.y);
    }

    void Update(){
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