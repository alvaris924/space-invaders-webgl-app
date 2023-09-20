using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For earlier prototype, let's do it in gamejam-way, with goal to realize features first, then come back for code optimization
/// </summary>
public class EnemySpawner : MonoBehaviour {

    public List<GameObject> EnemyPrefabs;

    public Vector2 GridSize = new Vector2(5, 3);
    public float spacing = 2.0f;
    public float startDelay = 2.0f;

    public Vector2 SpawnOffset;

    [ReadOnly]
    public List<GameObject> Enemies;

    public Transform SpawnParent;

    private void Awake() {
        
    }

    private void Start() {
        
    }

    [Button]
    public void SpawnEnemies() {

        DestroyAllEnemies();

        for (int x = 0; x < GridSize.x; x++) {

            for (int y = 0; y < GridSize.y; y++) {

                Vector3 spawnPosition = new Vector3((x + SpawnOffset.x) * spacing, (y + SpawnOffset.y) * spacing, 0);

                Instantiate(EnemyPrefabs[0], spawnPosition, Quaternion.identity, SpawnParent);

                Enemies.Add(EnemyPrefabs[0]);
            }

        }

    }

    [Button]
    public void DestroyAllEnemies() {
        foreach (GameObject enemy in Enemies) {
            Destroy(enemy);
        }

        // Clear the list to remove references to destroyed enemies
        Enemies.Clear();
    }

}
