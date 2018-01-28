using System;
using UnityEngine;

namespace LitterBox.Utils
{
  public static class TimeUtils
  {

    public static float ThrottledDelta(float maxStepTime)
    {
      return Mathf.Clamp(Time.deltaTime, 0f, maxStepTime);
    }
  }
}