using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> { 

    public AudioClip ShootClip;
    public AudioClip ExplosionClip;
    public AudioClip ShipExplosionClip;

    public AudioSource AudioSource;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.EnemyDestroyed, OnEnemyDestroyed);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    private void Start() {
        
    }

    void OnEnemyDestroyed(IMessage msg) {
        AudioSource.clip = ExplosionClip;
        AudioSource.Play();
    }

    void OnPlayerDefeated(IMessage msg) {
        AudioSource.clip = ShipExplosionClip;
        AudioSource.Play();
    }

}
    
