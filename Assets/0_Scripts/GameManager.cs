using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using com.ootii.Messages;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager> {

    public string PlayerName;

    [ReadOnly]
    public bool GameStarted;

    [ReadOnly]
    public bool GamePaused;

    public int CurrentPlayerLife = 3;

    public int KillCount;

    public int CurrentLevel;

    private void Awake() {
        Application.targetFrameRate = 60;

        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.PlayerAttacked, OnPlayerAttacked);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
        MessageDispatcher.AddListener(this, EventList.EnemyDestroyed, OnEnemyDestroyed);
        MessageDispatcher.AddListener(this, EventList.PlayerWon, OnPlayerWon);
    }

    private void Start() {
        
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnPlayerWon(IMessage msg) {
        
    }

    void OnEnemyDestroyed(IMessage msg) {
        KillCount++;
        if(KillCount == EnemySpawner.Instance.Enemies.Count) {
            MessageDispatcher.SendMessage(this, EventList.PlayerWon, null, 0);
        }
    }

    void OnGameStarted(IMessage msg) {
        CurrentPlayerLife = 3;
        KillCount = 0;
        GameStarted = true;
    }

    void OnPlayerAttacked(IMessage msg) {
        CurrentPlayerLife--;
        MessageDispatcher.SendMessage(this, EventList.PlayerStatUpdated, null, 0);
        if(CurrentPlayerLife <= 0) {
            MessageDispatcher.SendMessage(this, EventList.PlayerDefeated, null, 0);
        }
    }

    void OnPlayerDefeated(IMessage msg) {
        GameStarted = false;
        CurrentLevel = 0;
    }

    [Button]
    public void ToggleGamePause() {
        GamePaused = !GamePaused;

        if (GamePaused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// prototype entire game loop here
    /// </summary>
    public async UniTaskVoid StartGame() {

        await UniTask.Delay(TimeSpan.FromSeconds(0f));

        CurrentLevel++;

        MessageDispatcher.SendMessage(this, EventList.GameStarted, null, 0);

    }

    [Button]
    public void WinGame() {
        MessageDispatcher.SendMessage(this, EventList.PlayerWon, null, 0);
    }

    [Button]
    public void DeleteSaveData() {
        PlayerPrefs.DeleteAll();
    }
}
