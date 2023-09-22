using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour {

    public List<LeaderboardData> LeaderboardDatas = new List<LeaderboardData>();

    private void Awake() {
        LoadLeaderboard();
    }

    public void AddEntry(string playerName, int score) {

        LeaderboardData leaderboardData = new LeaderboardData {
            Name = playerName,
            Score = score
        };

        LeaderboardDatas.Add(leaderboardData);
        LeaderboardDatas.Sort((a, b) => b.Score.CompareTo(a.Score));
        SaveLeaderboard();
    }

    public void SaveLeaderboard() {
        string leaderboardData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("LeaderboardData", leaderboardData);
        PlayerPrefs.Save();
    }

    public void LoadLeaderboard() {
        if (PlayerPrefs.HasKey("LeaderboardData")) {
            string leaderboardData = PlayerPrefs.GetString("LeaderboardData");
            LeaderboardManager loadedLeaderboard = JsonUtility.FromJson<LeaderboardManager>(leaderboardData);
            LeaderboardDatas = loadedLeaderboard.LeaderboardDatas;
        }
    }
}
