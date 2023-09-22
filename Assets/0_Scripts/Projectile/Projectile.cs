
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField]
    internal OwnerTypes OwnerType;

    public Rigidbody Rigidbody;

    public PoolEntity PoolEntity;

    [SerializeField]
    [ReadOnly]
    private Vector2 screenPos;

    private CancellationTokenSource cancellationTokenSource;

    private void Start() {
        
    }

    private void Update() {

        screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPos.y >= (Screen.height) || screenPos.y <= 0) {
            if(!PoolEntity.IsAvailable) {
                Reset();
            }
        }

    }

    public void Reset() {

        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        PoolEntity.IsAvailable = true;

        CustomUtility.WaitBeforeAction(this, () => {
            OwnerType = OwnerTypes.None;
            gameObject.SetActive(false);
        }, 0.01f);
        
    }

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Enemy" && OwnerType == OwnerTypes.Player) {
            Reset();
        } else if (other.tag == "Player" && OwnerType == OwnerTypes.Enemy) {
            Reset();
        } else if (other.tag == "Projectile") {
            Reset();
        }
    }

}
