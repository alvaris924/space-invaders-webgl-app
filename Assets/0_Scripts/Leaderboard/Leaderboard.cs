using com.ootii.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Leaderboard {

    public List<LeaderboardData> LeaderboardDatas = new List<LeaderboardData>();

    public Leaderboard() {
        LeaderboardDatas = new List<LeaderboardData>();
    }

    public void AddEntry(string playerName, int score) {

        Debug.Log("AddEntry");

        LeaderboardData leaderboardData = new LeaderboardData {
            Name = playerName,
            Score = score
        };

        LeaderboardDatas.Add(leaderboardData);
        LeaderboardDatas.Sort((a, b) => b.Score.CompareTo(a.Score));
        LeaderboardManager.Instance.SaveLeaderboard();

        MessageDispatcher.SendMessage(this, EventList.LeaderboardUpdated, null, 0);
    }


}
