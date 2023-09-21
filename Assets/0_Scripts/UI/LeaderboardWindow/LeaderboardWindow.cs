using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardWindow : UIWindow {

    public Button ReturnButton;
    public Button ContinueButton;

    void Start() {

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

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }
}
