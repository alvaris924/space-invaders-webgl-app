using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardWindow : UIWindow {

    public List<LeaderboardItem> LeaderboardItems = new List<LeaderboardItem>();

    public Button ReturnButton;
    public Button ContinueButton;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.LeaderboardUpdated, OnLeaderboardUpdated);
    }

    void Start() {

        OnLeaderboardUpdated(null);

        MessageDispatcher.AddClickEvent(this, ReturnButton, () => {
            WindowManager.Instance.SetActiveAllWindows(false);
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Main, true);
        });

        MessageDispatcher.AddClickEvent(this, ContinueButton, () => {
            WindowManager.Instance.SetActiveAllWindows(false);
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Game, true);
            _ = GameManager.Instance.StartGame();
        });
    }

    private void OnEnable() {
        OnLeaderboardUpdated(null);
    }

    void OnLeaderboardUpdated(IMessage msg) {

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
