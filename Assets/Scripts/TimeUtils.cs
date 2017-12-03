using System;
using UnityEngine;

public static class TimeUtils {

  public static float ThrottledDelta(float maxStepTime){
    return Mathf.Clamp(Time.deltaTime, 0f, maxStepTime);
  }
}