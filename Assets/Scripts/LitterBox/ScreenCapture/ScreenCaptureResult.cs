using System;
using UnityEngine;

namespace LitterBox.ScreenCapture {

  public class ScreenCaptureResult: IScreenCaptureResult {
    public ScreenCaptureResult() {}
    public ScreenCaptureResult(Texture2D texture, string filePath) {
      this.ScreenTexture = texture;
      this.SavedFilePath = filePath;
    }
    
    public Texture2D ScreenTexture { get; set; }
    public string SavedFilePath {get; set;}
  }
}