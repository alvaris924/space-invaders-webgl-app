using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    public bool CanShoot;

    public GameObject ProjectilePrefab;

    public Transform ShootPoint;

    public Rigidbody Rigidbody;

    public float ProjectileSpeed;

    void Update() {

        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, 0f);

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Space)) {
            Shoot();
        }
    }

    [Button]
    public void Shoot() {

        GameObject projectileObject = PoolManager.Instance.SpawnGameObject(ProjectilePrefab, ShootPoint.position, ShootPoint.rotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        projectile.Rigidbody.AddForce(ShootPoint.forward * ProjectileSpeed, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag != "Projectile") {
            return;
        }

        gameObject.SetActive(false);
    }
}
