using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHolder : MonoBehaviour
{
    public GameObject Prefab;
    public List<GameObject> PooledGameObjects= new List<GameObject>();

    public PoolHolder(GameObject prefab) {
        Prefab = prefab;
    }
}
