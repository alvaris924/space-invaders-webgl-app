using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FieldManager : Singleton<FieldManager> {

    public Player Player;

    public GameObject ProjectilesParent;
    public GameObject ExplosionEffects_Parent;
    public GameObject Enemies_Parent;

    public Vector2 EdgeOffset = Vector2.zero;

    public float EnemyStartMoveSpeedMultiplier = 0.1f;
    public float EnemyMoveSpeedMultiplier = 0.05f;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.GameEnded, OnGameEnded);
        Player.gameObject.SetActive(false);
    }

    void OnGameStarted(IMessage msg) {
        //CurrentEnemyMoveSpeed = 0;
        Player.gameObject.SetActive(true);
        Player.PlayerController.Reset();
    }

    void OnGameEnded(IMessage msg) {
        //CurrentEnemyMoveSpeed = 0;
        Player.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

}
