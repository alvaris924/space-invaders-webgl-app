using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
public class PoolHolder {
    public GameObject Prefab;
    public List<GameObject> PooledGameObjects = new List<GameObject>();

    public PoolHolder(GameObject prefab) {
        Prefab = prefab;
    }
}
