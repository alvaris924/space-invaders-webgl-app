using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager> {

    public int Score;

    private void Awake() {

        MessageDispatcher.AddListener(this, EventList.EnemyDestroyed, OnEnemyDestroyed);

    }

    void OnEnemyDestroyed(IMessage msg) {

        int scorePoint = int.Parse(msg.Data.ToString());

        Score += scorePoint;

        MessageDispatcher.SendMessage(this, EventList.ScoreUpdated, Score, 0);

    }
}
