using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

public class PoolManager : Singleton<PoolManager> {

    [SerializeField][ReadOnly]
    private List<PoolHolder> pools = new List<PoolHolder>();

    private void Awake() {
        
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

        GameObject resultGameObject = poolHolder.PooledGameObjects.Find(x => !x.activeInHierarchy);

        if (resultGameObject == null) {
            resultGameObject = Instantiate(targetPrefab, targetPosition, targetRotation, targetParent);
            poolHolder.PooledGameObjects.Add(resultGameObject);
        }

        resultGameObject.SetActive(true);

        return resultGameObject;
    }

    public void DestroyGameObject(GameObject targetGameObject) {
        targetGameObject.SetActive(false);
    }


}
