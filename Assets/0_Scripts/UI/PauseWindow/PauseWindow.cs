using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : UIWindow {

    public Button SurrenderButton;
    public Button ContinueButton;

    private void Awake() {

        MessageDispatcher.AddClickEvent(this, SurrenderButton, () => {
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Pause, false);
            MessageDispatcher.SendMessage(this, EventList.PlayerDefeated, null, 0);
            GameManager.Instance.ToggleGamePause();
        });
        MessageDispatcher.AddClickEvent(this, ContinueButton, () => {
            //WindowManager.Instance.SetActiveWindow(UIWindowTypes.Game, true);
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Pause, false);
            GameManager.Instance.ToggleGamePause();
        });

    }

    private void Start() {
        
    }


}
