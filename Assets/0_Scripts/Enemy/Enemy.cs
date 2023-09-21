using com.ootii.Messages;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public bool CanMove = true;

    public float MoveSpeed = 1f;
    public float MoveDownSpeed = 1f;

    public Vector3 MoveDirection = Vector3.left;

    public Transform ShootPoint;

    public GameObject ProjectilePrefab;

    public float ProjectileSpeed = 1;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.TouchScreenEdge, OnTouchScreenEdge);
    }

    private void Start() {
        
    }

    void OnTouchScreenEdge(IMessage msg) {
        MoveDown();
    }

    void Update() {
        Move();
    }

    public void Move() {

        if (!CanMove) {
            return;
        }

        transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPos.x <= 0 || screenPos.x >= Screen.width) {
            MessageDispatcher.SendMessage(this, EventList.TouchScreenEdge, null, 0);
            //MoveDown();
        }

        if (transform.position.y < -10f) {
            Destroy(gameObject);
        }

    }

    public float MoveDownMultplier = 1;

    [Button]
    public void MoveDown(bool switchDir = false) {

        CanMove = false;

        Vector3 targetPosition = transform.position + (Vector3.down * MoveDownMultplier);

        transform.DOMove(targetPosition, 1.0f).OnComplete(() => {

            if(MoveDirection == Vector3.left) {
                MoveDirection = Vector3.right;
            } else {
                MoveDirection = Vector3.left;
            }

            CanMove = true;
        });

    }

    [Button]
    public void Shoot() {

        GameObject projectileObject = PoolManager.Instance.SpawnGameObject(ProjectilePrefab, ShootPoint.position, ShootPoint.rotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        projectile.OwnerType = OwnerTypes.Enemy;

        projectile.Rigidbody.AddForce(-ShootPoint.forward * ProjectileSpeed, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other) {

        Projectile projectile = other.GetComponent<Projectile>();

        if (projectile != null) {
            if(projectile.OwnerType == OwnerTypes.Player) {
                gameObject.SetActive(false);
            }
        } else {
            return;
        }

        
    }

}
