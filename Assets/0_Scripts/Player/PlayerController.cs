using com.ootii.Messages;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    public float moveSpeed = 5f;

    public bool CanShoot;

    public GameObject ProjectilePrefab;

    public Transform ShootPoint;

    public Rigidbody Rigidbody;

    public float ProjectileSpeed;

    public AudioSource AudioSource;

    public Vector2 EdgeOffset;

    public Renderer Renderer;

    private Vector2 screenBounds;
    private float rendererWidth;

    private void Awake() {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        rendererWidth = Renderer.bounds.size.x / 2;
    }

    void Update() {

        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, 0f);

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }

    }

    private void LateUpdate() {
        LimitEdgeMovement();
    }

    public void Reset() {
        Vector3 newPosition = transform.position;
        newPosition.x = 0f;
        transform.position = newPosition;
    }

    public void LimitEdgeMovement() {
        Vector3 viewPosition = transform.position;
        viewPosition.x = Mathf.Clamp(viewPosition.x, screenBounds.x + rendererWidth, screenBounds.x * -1 - rendererWidth);
        transform.position = viewPosition;
    }

    [Button]
    public void Shoot() {

        if(!GameManager.Instance.GameStarted) {
            return;
        }

        GameObject projectileObject = PoolManager.Instance.SpawnGameObject(ProjectilePrefab, ShootPoint.position, ShootPoint.rotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        projectile.OwnerType = OwnerTypes.Player;

        projectile.Rigidbody.AddForce(ShootPoint.forward * ProjectileSpeed, ForceMode.Impulse);

        AudioSource.clip = AudioManager.Instance.ShootClip;
        AudioSource.Play();
    }

    private void OnTriggerEnter(Collider other) {

        Projectile projectile = other.GetComponent<Projectile>();

        if (projectile != null) {
            if (projectile.OwnerType == OwnerTypes.Enemy) {
                MessageDispatcher.SendMessage(this, EventList.PlayerAttacked, null, 0);
            }
        } else {

        }

        if(other.tag == "Enemy") {
            Debug.Log("touched");
            // based on this video (https://www.youtube.com/watch?v=kR2fjwr-TzA), player will be directly defeated
            GameObject explosionObject = PoolManager.Instance.SpawnGameObject(VFXManager.Instance.ExplosionEffectPrefab, transform.position, transform.rotation);
            MessageDispatcher.SendMessage(this, EventList.PlayerDefeated, null, 1);
            MessageDispatcher.SendMessage(this, EventList.GameEnded, null, 1);
        }

    }

}
