using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    private enum State {
        WaitingToPlay,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
    private State state;

    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 60f;
    private bool isGamePause = false;

    private void Awake() {
        Instance = this;
        state = State.WaitingToPlay;
    }
    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteract += GameInput_OnInteract;
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        ToggleGamePause();
    }
    private void GameInput_OnInteract(object sender, EventArgs e) {
        if (state == State.WaitingToPlay) {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update() {
        switch (state) {
            case State.WaitingToPlay:
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0) {
                    gamePlayingTimer = gamePlayingTimerMax;
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);

                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0) {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }
    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }
    public bool IsGameOver() {
        return state == State.GameOver;
    }
    public float GetCountdownToStart() {
        return countdownToStartTimer;
    }
    public float GetGamePlayingTimerNormalize() {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }
    public void ToggleGamePause() {
        isGamePause = !isGamePause;
        if (isGamePause) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }
}


