using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class GameManager : Singleton<GameManager> {

    private void Awake() {
        Application.targetFrameRate = 60;

    }

    private void Start() {
        
    }

    /// <summary>
    /// prototype entire game loop here
    /// </summary>
    public async UniTaskVoid ExecuteGameLoop() {

        await UniTask.Delay(TimeSpan.FromSeconds(0f));

        // spawn enemy first
        EnemySpawner.Instance.SpawnEnemies();



    }
}
