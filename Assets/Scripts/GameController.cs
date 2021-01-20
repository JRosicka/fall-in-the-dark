﻿using System;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    private const string RESTART_NAME = "ctr_slow";

    public SongController SongController;
    public EnemyManager EnemyManager;
    public Transform ShotBucket;
    public WaypointManager WaypointManager;
    public TextMesh RestartText;
    public TextMesh SuccessText;
    public TimingController TimingController;
    public float DelaySecondsBeforeAllowedToRestart = 2f;
    
    public static GameController Instance;


    public Transform BoundaryRight;
    public Transform BoundaryLeft;
    public Transform BoundaryUp;
    public Transform BoundaryDown;

    private float xMax, xMin, yMax, yMin;

    private bool resettingGame;
    private float delaySecondsBeforeStart;
    private float elapsedStartDelayTime;
    private float elapsedResetTime;
    private bool won;
    private bool playedVictorySound;
    private bool started;
    
    private void Awake() {
        Instance = this;

        delaySecondsBeforeStart = TimingController.GetStartDelay();

        if (BoundaryRight != null)
            xMax = BoundaryRight.position.x;
        if (BoundaryLeft != null)
            xMin = BoundaryLeft.position.x;
        if (BoundaryUp != null)
            yMax = BoundaryUp.position.y;
        if (BoundaryDown != null)
            yMin = BoundaryDown.position.y;
    }

    private void Update() {
        if (resettingGame)
            HandleReset();
        else if (!started && elapsedStartDelayTime <= delaySecondsBeforeStart)
            elapsedStartDelayTime += Time.deltaTime;
        else if (!started) {
            started = true;
            SongController.PlaySong(); // TODO: Gross. Make this a pattern we schedule
        }
            
    }

    private void HandleReset() {
        elapsedResetTime += Time.deltaTime;

        // Fade out the music
        SongController.Music.volume -= .001f;
        
        if (!(elapsedResetTime > DelaySecondsBeforeAllowedToRestart)) 
            return;

        if (won) {
            if (SuccessText != null)
                SuccessText.gameObject.SetActive(true);
            if (!playedVictorySound) {
                SongController.PlayVictorySoundEffect();
                playedVictorySound = true;
            }
        } else if (RestartText != null) {
            RestartText.gameObject.SetActive(true);
        }

        Rewired.Player playerControls = ReInput.players.GetPlayer("SYSTEM");
        if (playerControls.GetButton(RESTART_NAME))
            SceneManager.LoadScene("MainScene");
    }

    public void ResetGame(bool won) {
        if (resettingGame)
            return;
        
        resettingGame = true;
        elapsedResetTime = 0;
        this.won = won;
    }

    public bool IsResetting() {
        return resettingGame;
    }

    public bool IsWaitingForStart() {
        return elapsedStartDelayTime < delaySecondsBeforeStart;
    }

    public Vector2 EvaluateMove(Vector2 originalMove, Vector2 currentPosition) {
        Vector2 finalMove = originalMove;
        if (currentPosition.y <= yMin)
            finalMove.y = Math.Max(originalMove.y, 0);
        else if (currentPosition.y >= yMax)
            finalMove.y = Math.Min(originalMove.y, 0);
        if (currentPosition.x <= xMin)
            finalMove.x = Math.Max(originalMove.x, 0);
        else if (currentPosition.x >= xMax)
            finalMove.x = Math.Min(originalMove.x, 0);
            
        return finalMove;
    }
}
