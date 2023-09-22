using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using com.ootii.Messages;

public class SecondProcessor : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    DateTime d = new DateTime();
    void FixedUpdate() {
        if ((DateTime.Now - d).TotalSeconds > 1) {
            d = DateTime.Now;
            MessageDispatcher.CallSecondTimer();
        }
    }
}
