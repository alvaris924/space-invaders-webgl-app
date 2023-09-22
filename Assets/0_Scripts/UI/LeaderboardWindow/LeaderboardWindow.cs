using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardWindow : UIWindow {

    public List<LeaderboardItem> LeaderboardItems = new List<LeaderboardItem>();

    public TextMeshProUGUI CurrentScoreText;

    public Button ReturnButton;
    public Button ContinueButton;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.LeaderboardUpdated, OnLeaderboardUpdated);

        ContinueButton.gameObject.SetActive(false);
    }

    void Start() {

        OnLeaderboardUpdated(null);

        MessageDispatcher.AddClickEvent(this, ReturnButton, () => {
            WindowManager.Instance.SetActiveAllWindows(false);
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Main, true);
            if(GameManager.Instance.CurrentLevel > 0) {
                //MessageDispatcher.SendMessage(this, EventList.PlayerDefeated, null, 1);
                MessageDispatcher.SendMessage(this, EventList.GameEnded, "Lose", 0);
            }
        });

        MessageDispatcher.AddClickEvent(this, ContinueButton, () => {
            WindowManager.Instance.SetActiveAllWindows(false);
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Game, true);
            GameManager.Instance.StartGame();
        });
    }

    private void OnEnable() {
        OnLeaderboardUpdated(null);
        ContinueButton.gameObject.SetActive(GameManager.Instance.CurrentLevel > 0);
    }

    void OnLeaderboardUpdated(IMessage msg) {

        CurrentScoreText.text = $"Current Score: {ScoreManager.Instance.Score}";

        for (int i = 0; i < LeaderboardItems.Count; i++) {
            if (LeaderboardManager.Instance.MainLeaderboard.LeaderboardDatas.Count > i) {
                LeaderboardItems[i].PlayerNameText.text = LeaderboardManager.Instance.MainLeaderboard.LeaderboardDatas[i].Name;
                LeaderboardItems[i].PlayerScoreText.text = LeaderboardManager.Instance.MainLeaderboard.LeaderboardDatas[i].Score.ToString();
            }
            else {
                LeaderboardItems[i].Reset();
            }
        }

    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }


}
