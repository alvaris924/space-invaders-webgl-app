using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour {

    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI PlayerScoreText;

    private void Awake() {
        Reset();
    }

    public void Reset() {
        PlayerNameText.text = "<Unregistered>";
        PlayerScoreText.text = "N/A";
    }
}
