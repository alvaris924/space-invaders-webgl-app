using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public OwnerTypes OwnerType;

    public Rigidbody Rigidbody;

    public PoolEntity PoolEntity;

    [SerializeField]
    [ReadOnly]
    private Vector2 screenPos;

    private void Start() {
        
    }

    private CancellationTokenSource cancellationTokenSource;

    private void Update() {

        screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPos.y >= (Screen.height) || screenPos.y <= 0) {
            if(!PoolEntity.IsAvailable) {
                _ = Reset();
            }
        }

    }

    public async UniTaskVoid Reset() {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        PoolEntity.IsAvailable = true;
        gameObject.SetActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        OwnerType = OwnerTypes.None;

    }

    private void OnTriggerEnter(Collider other) {

        _ = Reset();

        /*
        if (other.tag == "Enemy" && OwnerType == OwnerTypes.Player) {
            _ = Reset();
        }
        else if (other.tag == "Player" && OwnerType == OwnerTypes.Enemy) {
            _ = Reset();
        }
        */
    }

}
