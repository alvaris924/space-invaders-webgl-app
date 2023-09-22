using com.ootii.Messages;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CameraShake CameraShake;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.PlayerAttacked, OnPlayerAttacked);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    private void Start() {
        
    }

    void OnPlayerAttacked(IMessage msg) {
        CameraShake.Shake();
    }

    void OnPlayerDefeated(IMessage msg) {
        CameraShake.Shake();
    }
}
