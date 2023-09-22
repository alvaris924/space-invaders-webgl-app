using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : UIWindow {

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI StatusText;
    public Button PauseButton;
    public List<GameObject> LifeSprites;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.ScoreUpdated, OnScoreUpdated);
        MessageDispatcher.AddListener(this, EventList.PlayerStatUpdated, OnPlayerStatUpdated);
        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.GameEnded, OnGameEnded);

        MessageDispatcher.AddClickEvent(this, PauseButton, () => {
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Pause, true);
            GameManager.Instance.ToggleGamePause();
        });

        ScoreText.text = "0";
        StatusText.text = "";
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnGameStarted(IMessage msg) {
        StatusText.text = "";
    }

    void OnGameEnded(IMessage msg) {
        string status = msg.Data.ToString();
        if(status == "Win") {
            StatusText.text = "YOU WIN!";
        } else {
            StatusText.text = "YOU LOSE...";
        }
        ScoreText.text = ScoreManager.Instance.Score.ToString();
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
