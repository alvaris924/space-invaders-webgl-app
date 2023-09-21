using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainWindow : UIWindow
{
    public Button StartGameButton;

    private void Awake() {

        MessageDispatcher.AddClickEvent(this, StartGameButton, () => {
            gameObject.SetActive(false);
            GameManager.Instance.ExecuteGameLoop();
        });

    }
}
