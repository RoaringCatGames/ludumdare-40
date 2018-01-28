using System;
using UnityEngine;

namespace LitterBox.ScreenCapture {

  public interface IScreenCaptureResult {
    Texture2D ScreenTexture { get; }
    string SavedFilePath { get; }
  }
}