using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using com.ootii.Messages;

public class GameManager : Singleton<GameManager> {

    public bool GameStarted;

    public int CurrentPlayerLife = 3;

    private void Awake() {
        Application.targetFrameRate = 60;

        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.PlayerAttacked, OnPlayerAttacked);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    private void Start() {
        
    }

    void OnGameStarted(IMessage msg) {
        CurrentPlayerLife = 3;
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
    }

    /// <summary>
    /// prototype entire game loop here
    /// </summary>
    public async UniTaskVoid ExecuteGameLoop() {

        await UniTask.Delay(TimeSpan.FromSeconds(0f));

        MessageDispatcher.SendMessage(this, EventList.GameStarted, null, 0);

    }
}
