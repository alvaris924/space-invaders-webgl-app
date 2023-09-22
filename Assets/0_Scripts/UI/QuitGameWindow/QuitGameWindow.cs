using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameWindow : UIWindow {

    public Button YesButton;
    public Button NoButton;

    private void Awake() {

        MessageDispatcher.AddClickEvent(this, YesButton, () => {
            Application.Quit();
        });
        MessageDispatcher.AddClickEvent(this, NoButton, () => {
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.QuitGame, false);
        });

    }

}
