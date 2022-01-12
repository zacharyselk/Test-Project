using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOnTimer : MonoBehaviour {
  private System.Action callback;
  private float timeLeft = 0.0f;
  
  public void set(float time, System.Action cb) {
    timeLeft = time;
    callback = cb;
  }

  public void stop() {
    timeLeft = 0.0f;
  }

  private void Update() {
    if (timeLeft > 0) {
      timeLeft -= Time.deltaTime;
      if (timeLeft <= 0) {
        callback();
      }
    }
  }
}
