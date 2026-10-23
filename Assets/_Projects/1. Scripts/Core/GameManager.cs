using UnityEngine;
using UnityEngine.InputSystem;
using System;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> OnStateChanged;

    public enum GameState { Menu, Playing, Paused, GameOver }
    [ReadOnly] public GameState State;
    public bool skipIntro = true;

    [Header("CLOCK")]
    [Tooltip("Real-world minutes it takes for one in-game day (24 hours).")]
    [SerializeField] private float realMinutesPerInGameDay = 15f;

    private bool clockStarted;
    private float clockElapsedSeconds;

    void Awake(){
        if(Instance == null){ 
            Instance = this;
            return; 
        }else Destroy(gameObject); 
        return;
    }

    void Start(){
        ChangeState(skipIntro ? GameState.Playing : GameState.Menu);
    }

    void Update(){
        if(clockStarted && State == GameState.Playing)
            clockElapsedSeconds += Time.unscaledDeltaTime;

        if(Keyboard.current.escapeKey.wasPressedThisFrame){
            if(State == GameState.Playing) PauseGame();
            else if(State == GameState.Paused) ResumeGame();
        }
    }

    void ApplyPause(bool paused){
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }

    public void ChangeState(GameState newState){
        if(State == newState) return;
        var prev = State;
        State = newState;
        OnStateChanged?.Invoke(newState);

        if(newState == GameState.Playing && !clockStarted){
            clockStarted = true;
            clockElapsedSeconds = 0f;
        }
    }

    public void StartGame()  => ChangeState(GameState.Playing);
    public void ResumeGame() => ChangeState(GameState.Playing);
    public void PauseGame()  => ChangeState(GameState.Paused);
    public void GameOver()   => ChangeState(GameState.GameOver);

    public void QuitApplication(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void StartApplication(){
        ChangeState(GameState.Playing);
        //StartCoroutine(DialogueLibrary.Instance.Play(introDialogue));
    }

    public string GetClockString() => FormatClock(GetClockHours());
    public float GetClockHours(){
        if(realMinutesPerInGameDay <= 0f) return 0f;

        float realSecondsPerGameDay = realMinutesPerInGameDay * 60f;
        float realSecondsPerGameHour = realSecondsPerGameDay / 24f;
        return clockElapsedSeconds / realSecondsPerGameHour;
    }

    static string FormatClock(float hours){
        int totalMinutes = Mathf.FloorToInt(hours * 60f);
        int h = (totalMinutes / 60) % 24;
        int m = totalMinutes % 60;
        return $"{h:00}:{m:00}";
    }
}