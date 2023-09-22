using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager> {

    [ReadOnly]
    public int Score;

    private void Awake() {

        MessageDispatcher.AddListener(this, EventList.EnemyDestroyed, OnEnemyDestroyed);
        MessageDispatcher.AddListener(this, EventList.SaucerDestroyed, OnSaucerDestroyed);
        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.GameEnded, OnGameEnded);
        MessageDispatcher.AddListener(this, EventList.PlayerWon, OnPlayerWon);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnEnemyDestroyed(IMessage msg) {

        int scorePoint = int.Parse(msg.Data.ToString());
        Score += scorePoint;
        MessageDispatcher.SendMessage(this, EventList.ScoreUpdated, Score, 0);

    }

    void OnSaucerDestroyed(IMessage msg) {

        int scorePoint = int.Parse(msg.Data.ToString());
        Score += scorePoint;
        MessageDispatcher.SendMessage(this, EventList.ScoreUpdated, Score, 0);

    }

    void OnGameStarted(IMessage msg) {
        if(GameManager.Instance.SessionResultType == GameSessionResultTypes.Lose) {
            Score = 0;
        }
    }

    void OnGameEnded(IMessage msg) {
        
    }

    void OnPlayerWon(IMessage msg) {

    }

    void OnPlayerDefeated(IMessage msg) {
        
    }
}
