using com.ootii.Messages;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed = 5f;

    public bool CanShoot;

    public GameObject ProjectilePrefab;

    public Transform ShootPoint;

    public Rigidbody Rigidbody;

    public float ProjectileSpeed;

    public AudioSource AudioSource;

    void Update() {

        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, 0f);

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }

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
            MessageDispatcher.SendMessage(this, EventList.PlayerAttacked, null, 0);
        }
    }

}
