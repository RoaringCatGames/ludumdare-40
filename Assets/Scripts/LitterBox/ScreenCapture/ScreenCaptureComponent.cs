using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace LitterBox.ScreenCapture
{

  public class ScreenCaptureComponent : MonoBehaviour
  {
    [Tooltip("Determines whether all Canvas objects should be hidden or not before capturing the screenshot")]
    public bool hideUIOnScreenCapture = true;
    [Tooltip("A relative file path to the Application.persistentDataPath into which the screenshot should be saved")]
    public string relativeFileSavePath = null;
    [Tooltip("The saved filename with a placeholder for a timestamp. Example: screenshot-{0}.png")]
    public string savedFileNameFormatted = null;

    public ScreenCaptureComplete onScreenCaptureComplete;
    public ScreenCaptureSaved onScreenCaptureSaved;

    public void CaptureScreen()
    {
      StartCoroutine(ScreenCaptureUtil.CaptureScreenShotAsTexture(HandleScreenCaputure, hideUIOnScreenCapture, relativeFileSavePath, savedFileNameFormatted));
    }

    private void HandleScreenCaputure(IScreenCaptureResult result)
    {
      this.onScreenCaptureComplete.Invoke(result.ScreenTexture);

      if (!string.IsNullOrEmpty(result.SavedFilePath))
      {
        this.onScreenCaptureSaved.Invoke(result.SavedFilePath);
      }
    }
  }
}
