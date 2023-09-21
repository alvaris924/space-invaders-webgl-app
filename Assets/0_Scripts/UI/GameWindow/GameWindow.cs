using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameWindow : UIWindow {

    public TextMeshProUGUI ScoreText;

    public List<GameObject> LifeSprites;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.ScoreUpdated, OnScoreUpdated);
        MessageDispatcher.AddListener(this, EventList.PlayerStatUpdated, OnPlayerStatUpdated);
        ScoreText.text = "0";
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnScoreUpdated(IMessage msg) {
        int score = int.Parse(msg.Data.ToString());

        ScoreText.text = score.ToString();
    }

    void OnPlayerStatUpdated(IMessage msg) {
        SetLife(GameManager.Instance.CurrentPlayerLife);
    }

    public void SetLife(int life) {
        LifeSprites.ForEach(sprite => { sprite.SetActive(false); });
        for (int i = 0; i < life; i++) {
            LifeSprites[i].SetActive(true);
        }
    }

}
