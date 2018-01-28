using System;
using UnityEngine;
using UnityEngine.Events;

namespace LitterBox.ScreenCapture
{
  [Serializable]
  public class ScreenCaptureComplete : UnityEvent<Texture2D> { }

  [Serializable]
  public class ScreenCaptureSaved : UnityEvent<string> { }
}