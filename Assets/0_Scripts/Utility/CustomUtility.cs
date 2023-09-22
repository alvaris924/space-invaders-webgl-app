using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomUtility {
    public static void WaitBeforeAction(MonoBehaviour parent, Action cb, float delayTime) {
        MessageDispatcher.BeginCoroutine(parent, DelayTime(cb, delayTime));
    }
    public static IEnumerator DelayTime(Action cb, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        cb();
    }

    public static void WaitBeforeAction(MonoBehaviour parent, Action cb, bool condition, float delayTime) {
        MessageDispatcher.BeginCoroutine(parent, IE_WaitBeforeAction(cb, condition, delayTime));
    }

    public static IEnumerator IE_WaitBeforeAction(Action cb, bool condition, float delayTime) {
        while (!condition) {
            yield return new WaitForSeconds(delayTime);
        }
        cb();
    }
}
