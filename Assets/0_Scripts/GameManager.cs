using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2 Edge;

    private void Awake() {
        Application.targetFrameRate = 60;

    }
}
