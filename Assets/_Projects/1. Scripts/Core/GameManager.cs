using UnityEngine;
using UnityEngine.InputSystem;
using System;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, Paused, GameOver }
    [ReadOnly] public GameState State;
    public event Action<GameState> OnStateChanged;

    void Awake(){
        if(Instance == null){ 
            Instance = this;
            return; 
        }else Destroy(gameObject); 
        return;
    }

    void Start(){
        ChangeState(GameState.Menu);
    }

    void Update(){
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
    }

    public void StartGame()  => ChangeState(GameState.Playing);
    public void ResumeGame() => ChangeState(GameState.Playing);
    public void PauseGame()  => ChangeState(GameState.Paused);
    public void GameOver()   => ChangeState(GameState.GameOver);
}