using com.ootii.Messages;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainWindow : UIWindow
{
    public Button LeaderboardButton;
    public Button StartGameButton;

    public Button QuitGameButton;

    public TextMeshProUGUI PlayerNameText;

    [Title("Name Change Section")]
    public Button NameChangeButton;

    public GameObject NameChangeSection;
    public TMP_InputField NameChangeInputField;
    public Button ChangeNameButton;

    private void Awake() {

        MessageDispatcher.AddClickEvent(this, LeaderboardButton, () => {
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.Leaderboard, true);
        });

        MessageDispatcher.AddClickEvent(this, QuitGameButton, () => {
            WindowManager.Instance.SetActiveWindow(UIWindowTypes.QuitGame, true);
        });

        MessageDispatcher.AddClickEvent(this, StartGameButton, () => {
            gameObject.SetActive(false);
            _ = GameManager.Instance.StartGame();
        });

        MessageDispatcher.AddClickEvent(this, NameChangeButton, () => {
            NameChangeSection.SetActive(true);
            NameChangeInputField.text = UserManager.Instance.Username;
        });

        MessageDispatcher.AddClickEvent(this, ChangeNameButton, () => {
            UserManager.Instance.Username = NameChangeInputField.text;
            PlayerNameText.text = NameChangeInputField.text;
            NameChangeSection.SetActive(false);
            UserManager.Instance.SaveUserName();
        });

        MessageDispatcher.AddListener(this, EventList.UserStatUpdated, OnUserStatUpdated);

    }

    void OnUserStatUpdated(IMessage msg) {
        PlayerNameText.text = UserManager.Instance.Username;
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }
}
