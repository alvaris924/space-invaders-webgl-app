using com.ootii.Messages;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// For earlier prototype, let's do it in gamejam-way, with goal to realize features first, then come back for code optimization
/// </summary>
public class EnemySpawner : Singleton<EnemySpawner> {

    public List<GameObject> EnemyPrefabs;

    public Vector2 GridSize = new Vector2(5, 3);
    public float spacing = 2.0f;
    public float startDelay = 2.0f;

    public Vector2 SpawnOffset;

    private Vector2 StartingSpawnOffset;

    [ReadOnly]
    public List<Enemy> Enemies;

    public Transform SpawnParent;

    public List<Enemy> GetAliveEnemies() {
        return Enemies.Where(enemy => !enemy.PoolEntity.IsAvailable).ToList();
    }
    public List<Enemy> GetDeadEnemies() {
        return Enemies.Where(enemy => enemy.PoolEntity.IsAvailable).ToList();
    }

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
        MessageDispatcher.AddListener(this, EventList.PlayerWon, OnPlayerWon);

        StartingSpawnOffset = SpawnOffset;
    }

    private void Start() {
        
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnGameStarted(IMessage msg) {
        // spawn enemy first
        
        SpawnOffset.y = Mathf.Max(StartingSpawnOffset.y - ((GameManager.Instance.CurrentLevel-1) * 1f), -2f);

        Debug.Log(SpawnOffset.y);

        SpawnEnemies();
    }

    void OnPlayerDefeated(IMessage msg) {
        DestroyAllEnemies();
    }

    void OnPlayerWon(IMessage msg) {
        DestroyAllEnemies();
    }

    public void Reset() {
        SpawnOffset.y = StartingSpawnOffset.y;
    }

    [Button]
    public void SpawnEnemies() {

        DestroyAllEnemies();

        int gameLevelIndex = Mathf.Min(GameManager.Instance.CurrentLevel-1, GameLevelManager.Instance.Levels.Count-1);
        if(gameLevelIndex < 0) {
            gameLevelIndex = 0;
        }

        for (int x = 0; x < GridSize.x; x++) {

            for (int y = 0; y < GridSize.y; y++) {

                int enemyPrefabIndex = GameLevelManager.Instance.Levels[gameLevelIndex].EnemyIndexByRows[y];

                Vector3 spawnPosition = new Vector3((x + SpawnOffset.x) * spacing, (y + SpawnOffset.y) * spacing, 0);

                GameObject enemyObject = PoolManager.Instance.SpawnGameObject(EnemyPrefabs[enemyPrefabIndex], spawnPosition, Quaternion.identity, SpawnParent);

                Enemy enemy = enemyObject.GetComponent<Enemy>();

                enemy.Init();

                Enemies.Add(enemy);

            }

        }

    }

    [Button]
    public void DestroyAllEnemies() {

        /*
        foreach (Enemy enemy in Enemies) {
            Destroy(enemy.gameObject);
        }
        */

        // Clear the list to remove references to destroyed enemies
        Enemies.Clear();
    }

}
