using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FieldManager : Singleton<FieldManager> {

    public Vector2 EdgeOffset = Vector2.zero;

    public float EnemyMoveSpeedMultiplier = 1f;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
    }

    void OnGameStarted(IMessage msg) {
        //CurrentEnemyMoveSpeed = 0;
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

}
