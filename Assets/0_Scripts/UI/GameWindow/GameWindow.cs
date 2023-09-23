using com.ootii.Messages;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : UIWindow {

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI StatusText;
    public Button PauseButton;
    public List<GameObject> LifeSprites;
    public TextMeshProUGUI LevelText;

    [SerializeField]
    [ReadOnly]
    private long CountdownNumber;

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
        LevelText.text = "LEVEL 1";
        PauseButton.gameObject.SetActive(false);
    }

    private void OnEnable() {
        StatusText.text = "";
        if (GameManager.Instance.SessionResultType == GameSessionResultTypes.Lose) {
            ScoreText.text = "0";
            //LevelText.text = "LEVEL 1";
        } else {
            
        }
        LevelText.text = $"LEVEL: {GameManager.Instance.CurrentLevel}";
        SetLife(3);
        PauseButton.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnGameStarted(IMessage msg) {
        StatusText.text = "";
        PauseButton.gameObject.SetActive(true);
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
        try {
            LifeSprites.ForEach(sprite => { sprite.SetActive(false); });
            for (int i = 0; i < life; i++) {
                LifeSprites[i].SetActive(true);
            }
        } catch(Exception ex) {
            Debug.LogError($"Can't set life sprite, reason {ex}");
        }
    }

    [Button]
    public void CountStartGame() {

        IObservable<long> numberStream = Observable.Interval(TimeSpan.FromSeconds(1));

        numberStream
            .Take(3)
            .ObserveOnMainThread() 
            .Subscribe(number => {
                CountdownNumber = number;
                long reverseNumber = 2 - number;
                if(number < 2) {
                    StatusText.text = $"Starts in {reverseNumber}";
                }
                if(number >= 2) {
                    MessageDispatcher.SendMessage(this, EventList.GameStarted, null, 0);
                }
            })
            .AddTo(this);
    }

}
