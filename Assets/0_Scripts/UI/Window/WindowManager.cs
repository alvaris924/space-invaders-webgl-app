using com.ootii.Messages;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindowManager : Singleton<WindowManager> {

    public List<UIWindow> Windows;

    public UIWindow GetWindowByType(UIWindowTypes windowType) {
        return Windows.FirstOrDefault(uiWindow => uiWindow.WindowType == windowType);
    }
    private void Awake() {
        MessageDispatcher.AddListener(this, EventList.GameCountdown, OnGameCountdown);
        MessageDispatcher.AddListener(this, EventList.GameStarted, OnGameStarted);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
        MessageDispatcher.AddListener(this, EventList.PlayerWon, OnPlayerWon);
    }

    private void Start() {
        SetActiveAllWindows(false);
        SetActiveWindow(UIWindowTypes.Main, true);
    }

    private void OnDestroy() {
        MessageDispatcher.RemoveAllListenersFromParent(this);
    }

    void OnGameCountdown(IMessage msg) {
        SetActiveWindow(UIWindowTypes.Game, true);
        SetActiveWindow(UIWindowTypes.Main, false);
        GetWindowByType(UIWindowTypes.Game).GetComponent<GameWindow>().CountStartGame();
    }

    void OnGameStarted(IMessage msg) {

    }

    void OnPlayerDefeated(IMessage msg) {
        //Debug.Log("OnPlayerDefeated");
        SetActiveWindow(UIWindowTypes.Game, false);
        SetActiveWindow(UIWindowTypes.Leaderboard, true);
    }

    void OnPlayerWon(IMessage msg) {
        //Debug.Log("OnPlayerWon");
        SetActiveWindow(UIWindowTypes.Game, false);
        SetActiveWindow(UIWindowTypes.Leaderboard, true);
    }

    [Button]
    public void SetActiveWindow(UIWindowTypes windowType, bool active = true) {
        GetWindowByType(windowType).gameObject.SetActive(active);
    }

    [Button]
    public void SetActiveAllWindows(bool active) {
        Windows.ForEach(window => window.gameObject.SetActive(active));
    }

}
