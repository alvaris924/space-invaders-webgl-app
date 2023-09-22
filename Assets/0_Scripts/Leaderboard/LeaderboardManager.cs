using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : Singleton<LeaderboardManager> {

    public Leaderboard MainLeaderboard;

    private void Awake() {
        LoadLeaderboard();
        MessageDispatcher.AddListener(this, EventList.PlayerWon, OnPlayerWon);
        MessageDispatcher.AddListener(this, EventList.PlayerDefeated, OnPlayerDefeated);
    }

    void OnPlayerWon(IMessage msg) {
        MainLeaderboard.AddEntry(UserManager.Instance.Username, ScoreManager.Instance.Score);
        MessageDispatcher.SendMessage(this, EventList.LeaderboardUpdated, null, 0);
    }

    void OnPlayerDefeated(IMessage msg) {
        MainLeaderboard.AddEntry(UserManager.Instance.Username, ScoreManager.Instance.Score);
        MessageDispatcher.SendMessage(this, EventList.LeaderboardUpdated, null, 0);
    }

    public void SaveLeaderboard() {
        string leaderboardData = JsonUtility.ToJson(MainLeaderboard);
        PlayerPrefs.SetString("LeaderboardData", leaderboardData);
        PlayerPrefs.Save();
    }

    public void LoadLeaderboard() {
        if (PlayerPrefs.HasKey("LeaderboardData")) {

            string leaderboardData = PlayerPrefs.GetString("LeaderboardData");
            Debug.Log(leaderboardData);

            Leaderboard loadedLeaderboard = JsonUtility.FromJson<Leaderboard>(leaderboardData);

            Debug.Log(loadedLeaderboard.LeaderboardDatas.Count);

            MainLeaderboard = loadedLeaderboard;

            MessageDispatcher.SendMessage(this, EventList.LeaderboardUpdated, null, 0);

        }
    }
}
