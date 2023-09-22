using com.ootii.Messages;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : Singleton<UserManager> {

    public string Username = "Amazing Player";

    private void Awake() {
        
    }

    private void Start() {
        LoadUserName();
        Username = WindowManager.Instance.Windows[(int)UIWindowTypes.Main].GetComponent<MainWindow>().PlayerNameText.text;
    }

    public void SaveUserName() {
        PlayerPrefs.SetString("Username", Username);
    }

    [Button]
    public void LoadUserName() {
        Username = PlayerPrefs.GetString("Username");
        MessageDispatcher.SendMessage(this, EventList.UserStatUpdated, null, 0);
    }
}
