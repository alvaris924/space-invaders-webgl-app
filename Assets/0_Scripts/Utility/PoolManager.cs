using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using com.ootii.Messages;

public class PoolManager : Singleton<PoolManager> {

    [ReadOnly]
    public List<PoolHolder> pools = new List<PoolHolder>();

    private void Awake() {
        ClearPools();

        MessageDispatcher.AddListener(this, EventList.PlayerWon, OnPlayerWon);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnPlayerWon(IMessage msg) {
        DestroyAllPools();
        ClearPools();
    }

    void OnPlayerDefeated(IMessage msg) {
        DestroyAllPools();
        ClearPools();
    }

    public GameObject SpawnGameObject(
        GameObject targetPrefab, 
        Vector3 targetPosition = default(Vector3), 
        Quaternion targetRotation = default(Quaternion), 
        Transform targetParent = null
    ) {

        PoolHolder poolHolder = pools.FirstOrDefault(x => x.Prefab == targetPrefab);

        if (poolHolder == null) {
            poolHolder = new PoolHolder(targetPrefab);
            pools.Add(poolHolder);
        }

        GameObject resultGameObject = poolHolder.PooledGameObjects.Find(x => x.GetComponent<PoolEntity>().IsAvailable);

        if (resultGameObject == null) {
            resultGameObject = Instantiate(targetPrefab, targetPosition, targetRotation, targetParent);
            poolHolder.PooledGameObjects.Add(resultGameObject);
            
        } else {
            resultGameObject.transform.position = targetPosition;
            resultGameObject.transform.rotation = targetRotation;
            resultGameObject.transform.parent = targetParent;
        }

        resultGameObject.GetComponent<PoolEntity>().IsAvailable = false;
        resultGameObject.SetActive(true);

        return resultGameObject;
    }

    public void DestroyGameObject(GameObject targetGameObject) {
        targetGameObject.SetActive(false);
    }

    [Button]
    public void ClearPools() {
        pools.Clear();
    }

    public void DestroyAllPools() {

        for (int i = 0; i < pools.Count; i++) {

            for (int j = pools[i].PooledGameObjects.Count - 1; j >= 0; j--) {
                Destroy(pools[i].PooledGameObjects[j]);
            }

            pools[i].PooledGameObjects.Clear();
        }

    }
}
