using com.ootii.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindowManager : Singleton<WindowManager> {

    public List<UIWindow> Windows;

    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    private void Start() {
        
    }

    void OnPlayerDefeated(IMessage msg) {
        SetActiveWindow(UIWindowTypes.Game, false);
        SetActiveWindow(UIWindowTypes.Main, false);
    }

    public UIWindow GetWindowByType(UIWindowTypes windowType) {
        return Windows.FirstOrDefault(uiWindow => uiWindow.WindowType == windowType);
    }

    public void SetActiveWindow(UIWindowTypes windowType, bool active) {
        GetWindowByType(windowType).gameObject.SetActive(active);
    }

}
