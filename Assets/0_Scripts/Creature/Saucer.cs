using com.ootii.Messages;
using Sirenix.OdinInspector;
using UnityEngine;

public class Saucer : MonoBehaviour {

    public int ScorePoint = 100;

    [Title("Movement")]
    public float MoveSpeed = 3;
    public float MoveCountdown = 5;

    [SerializeField][ReadOnly]
    private bool canMove = false;
    
    [Title("Renderer")]
    public Renderer Renderer;

    private float screenWidth;

    private void Awake() {
        Reset();

        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.GameEnded, OnGameEnded);
        MessageDispatcher.AddListener(this, EventList.PlayerWon, OnPlayerWon);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    private void Start() {

        // Calculate the screen width in world coordinates
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        screenWidth = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distanceToCamera)).x;

        MessageDispatcher.AddSecondTimer(this, () => {
            if (GameManager.Instance.GameStarted) {
                MoveCountdown--;
                if (MoveCountdown <= 0) {
                    canMove = true;
                }
            }
        });
    }

    void OnGameStarted(IMessage msg) {
        Reset();
    }
    void OnGameEnded(IMessage msg) {
        Reset();
    }
    void OnPlayerWon(IMessage msg) {
        Reset();
    }
    void OnPlayerDefeated(IMessage msg) {
        Reset();
    }

    public void Reset() {
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        screenWidth = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distanceToCamera)).x;
        float newPositionX = screenWidth * 2f;
        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);

        MoveCountdown = 5;
        canMove = false;
    }

    private void Update() {
        Move();
    }

    [Button]
    private void Move() {
        if(!canMove) {
            return;
        }

        // Move the GameObject to the left
        transform.Translate(Vector3.left * MoveSpeed * Time.deltaTime);

        // If the GameObject goes beyond the left edge, reset its position to the right edge
        if (transform.position.x < -screenWidth) {
            float newPositionX = screenWidth * 2f;
            transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);
            canMove = false;
            MoveCountdown = 5;
        }
    }

    private void OnTriggerEnter(Collider other) {

        Projectile projectile = other.GetComponent<Projectile>();

        if (projectile != null) {
            if (projectile.OwnerType == OwnerTypes.Player) {
                GameObject explosionObject = PoolManager.Instance.SpawnGameObject(VFXManager.Instance.ExplosionEffectPrefab, transform.position, transform.rotation);
                Reset();
                MessageDispatcher.SendMessage(this, EventList.SaucerDestroyed, ScorePoint, 0);
            }
        } else {

        }
    }



}
