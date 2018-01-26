using System;
using UnityEngine;

namespace LitterBox.Utils {

  public interface IScreenCaptureResult {
    Texture2D ScreenTexture { get; }
    string SavedFilePath { get; }
  }
}