using com.ootii.Messages;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public bool CanMove = true;
    public bool CanMoveDown = true;

    public bool CanShoot = true;

    public float MoveSpeed = 1f;
    public float MoveDownSpeed = 1f;

    public Vector3 MoveDirection = Vector3.left;

    public Transform ShootPoint;

    public GameObject ProjectilePrefab;

    public float ProjectileSpeed = 1;

    public int ScorePoint = 100;

    public Vector2 EdgeOffset = Vector2.zero;

    public Vector2 AttackInterval = new Vector2(3, 10);

    [SerializeField]
    [ReadOnly]
    private Vector2 screenPos;

    public PoolEntity PoolEntity;

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

    public float RaycastDistance = 1.0f;

    [Button]
    public bool HasFriendOnFront() {

        RaycastHit hit;
        Vector3 raycastDirection = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, raycastDirection, out hit, RaycastDistance)) {
            if(hit.transform.tag == "Enemy") {
                return true;
            }
        }

        return false;
    }

    public void Move() {

        if (!CanMove) {
            return;
        }

        transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime);

        screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPos.x <= (0 + FieldManager.Instance.EdgeOffset.x) ||
            screenPos.x >= (Screen.width + FieldManager.Instance.EdgeOffset.y)
        ) {
            if(CanMove) {
                MessageDispatcher.SendMessage(this, EventList.TouchScreenEdge, null, 0);
            }
        }

        if (transform.position.y < -5.5f) {
            MessageDispatcher.SendMessage(this, EventList.PlayerDefeated, null, 0);
            //Destroy(gameObject);
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

    private CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// Attack with interval
    /// </summary>
    [Button]
    public void StartAttack() {
        cancellationTokenSource = new CancellationTokenSource();
        _ = Attack(cancellationTokenSource.Token);
    }

    [Button]
    public void StopAttack() {
        if (cancellationTokenSource != null) {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }

    public async UniTaskVoid Attack(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(AttackInterval.x, AttackInterval.y)), cancellationToken: cancellationToken);
            Shoot();
        }
    }

    [Button]
    public void Shoot() {

        if (HasFriendOnFront()) {
            return;
        }

        if (!CanShoot) {
            return;
        }

        GameObject projectileObject = PoolManager.Instance.SpawnGameObject(ProjectilePrefab, ShootPoint.position, ShootPoint.rotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        projectile.OwnerType = OwnerTypes.Enemy;

        projectile.Rigidbody.AddForce(-ShootPoint.forward * ProjectileSpeed, ForceMode.Impulse);

    }

    public void Init() {
        gameObject.SetActive(true);
        CanMove = true;
        StartAttack();
    }

    public void Reset() {
        gameObject.SetActive(false);
        CanMove = false;
        StopAttack();
    }

    private void OnTriggerEnter(Collider other) {

        Projectile projectile = other.GetComponent<Projectile>();

        if (projectile != null) {
            if(projectile.OwnerType == OwnerTypes.Player) {
                Reset();
                MessageDispatcher.SendMessage(this, EventList.EnemyDestroyed, ScorePoint, 0);
            }
        } else {

        }

        if(other.tag == "Player") {
            // based on this video (https://www.youtube.com/watch?v=kR2fjwr-TzA), player will be directly defeated
            MessageDispatcher.SendMessage(this, EventList.PlayerDefeated, ScorePoint, 0);
        }
    }

}
