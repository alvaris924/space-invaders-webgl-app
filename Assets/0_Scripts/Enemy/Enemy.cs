using com.ootii.Messages;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PoolEntity))]
public class Enemy : MonoBehaviour {

    [Title("Movement")]
    public bool CanMove = true;

    public bool CanShoot = true;

    //[ReadOnly]
    public float MoveSpeed = 2f;

    [ReadOnly]
    public float MoveSpeedMultiplier = 1f;

    //[ReadOnly]
    public float MoveDownSpeed = 2f;

    [ReadOnly]
    public Vector3 MoveDirection = Vector3.left;

    public float MoveDownMultplier = 1;

    [InfoBox("Needs to be false initially for enemy to move")]
    public bool CanMoveDown = true;


    [Title("Attack")]
    public float ProjectileSpeed = 3;

    public Vector2 AttackInterval = new Vector2(3, 10);

    [Title("Score")]
    public int ScorePoint = 100;

    [Title("Physics")]
    public float RaycastDistance = 10.0f;

    [SerializeField]
    [ReadOnly]
    private Vector2 screenPos;

    [Title("Reference")]
    public Transform ShootPoint;

    public GameObject ProjectilePrefab;

    public PoolEntity PoolEntity;

    [Title("Audio")]
    public AudioSource AudioSource;

    private Tween LerpPositionSequence;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.TouchScreenEdge, OnTouchScreenEdge);
        MessageDispatcher.AddListener(this, EventList.EnemyDestroyed, OnEnemyDestroyed);

    }

    private void Start() {
        MoveSpeedMultiplier += Mathf.Min(1, ((GameManager.Instance.CurrentLevel) * FieldManager.Instance.EnemyStartMoveSpeedMultiplier));
    }


    void FixedUpdate() {
        Move();
    }

    private void OnDestroy() {
        StopAttack();
        StopAllCoroutines();
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnTouchScreenEdge(IMessage msg) {
        MoveDown();
    }

    void OnEnemyDestroyed(IMessage msg) {
        MoveSpeedMultiplier += FieldManager.Instance.EnemyMoveSpeedMultiplier; 
    }

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

        screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (!CanMove || CanMoveDown) {
            return;
        }

        transform.Translate(MoveDirection * MoveSpeed * MoveSpeedMultiplier * Time.deltaTime);

        if ((screenPos.x <= (0 + FieldManager.Instance.EdgeOffset.x) && MoveDirection == Vector3.left) ||
            (screenPos.x >= (Screen.width + FieldManager.Instance.EdgeOffset.y) && MoveDirection == Vector3.right)
        ) {
            if(CanMove) {
                Debug.Log("True, MoveDirection: " + MoveDirection);
                if(!CanMoveDown) {
                    MessageDispatcher.SendMessage(this, EventList.TouchScreenEdge, null, 0);
                }
            }
        }

    }

    [Button]
    public void MoveDown() {
        StopCoroutine("IE_MoveDown");
        StartCoroutine("IE_MoveDown");
    }

    private IEnumerator IE_MoveDown() {

        CanMove = false;
        CanMoveDown = true;

        yield return new WaitForSeconds(0f);

        // Debug.Log("MoveDown");

        if (CanMoveDown) {

            CanMoveDown = false;

            Vector3 targetPosition = transform.position + (Vector3.down * MoveDownMultplier);

            LerpPositionSequence.Kill(false);

            // stop this too
            LerpPositionSequence = transform.DOMove(targetPosition, 1.0f).OnComplete(() => {

                if (MoveDirection == Vector3.left) {
                    MoveDirection = Vector3.right;
                } else {
                    MoveDirection = Vector3.left;
                }

                CanMove = true;

            });

        }


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

        //AudioSource.clip = AudioManager.Instance.ShootClip;
        //AudioSource.Play();
    }

    public void Init() {
        gameObject.SetActive(true);
        CanMove = true;
        StartAttack();
    }

    public void Reset() {
        gameObject.SetActive(false);
        CanMove = false;
        CanMoveDown = false;
        StopAttack();
    }

    private void OnTriggerEnter(Collider other) {

        Projectile projectile = other.GetComponent<Projectile>();

        if (projectile != null) {
            if(projectile.OwnerType == OwnerTypes.Player) {
                GameObject explosionObject = PoolManager.Instance.SpawnGameObject(VFXManager.Instance.ExplosionEffectPrefab, transform.position, transform.rotation);
                //AudioSource.clip = AudioManager.Instance.ExplosionClip;
                //AudioSource.Play();
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
