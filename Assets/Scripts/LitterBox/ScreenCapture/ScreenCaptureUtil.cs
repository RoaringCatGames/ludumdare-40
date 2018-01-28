using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LitterBox.ScreenCapture {

  public delegate void ScreenCaptureResultHandler(IScreenCaptureResult result);
  public class ScreenCaptureUtil {

    /// <summary>
    /// Take a given filename and add a format token before the fileExtension
    /// </summary>
    /// <param name="fileName">The file name without a token included</param>
    /// <param name="extensionType">The file extension type. Defaults to ".png"</param>
    /// <returns></returns>
    public static string FileNameToFormatString(string fileName, string extensionType = ".png") {
      if(string.IsNullOrEmpty(fileName)) {
        return string.Concat("default-file-name-{0}", extensionType);
      }
      var extensionIndex = fileName.LastIndexOf(".");
      if(extensionIndex < 0){
        return string.Concat(fileName, "-{0}", extensionType);
      }else{
        return string.Concat(fileName.Substring(0, extensionIndex - 1), "-{0}", fileName.Substring(extensionIndex));
      }
    }

    /// <summary>
    /// Capture a ScreenShot Asynchronously with various options. By default the screenshot will just
    ///  be saved to a Texture2D and provided to the callback. Use the optional params to do more.
    /// </summary>
    /// <param name="callback">A delegate to handle the resulting screenshot result</param>
    /// <param name="hideUI">A boolean that will hide all Canvas objects before capturing the screenshot.</param>
    /// <param name="relativeFilePath">Provide a file path relative to the application to save to</param>
    /// <param name="fileSaveNameFormat">A file name with a single placeholder for a timestamp. Example: mygame-screenshot-{0}.png</param>
    /// <returns>This method returns an IEnumerator and is meant to be run in a Coroutine and provided a callback to handle the result.</returns>
    public static IEnumerator CaptureScreenShotAsTexture(ScreenCaptureResultHandler callback, bool hideUI = true, string relativeFilePath = null, string fileSaveNameFormat = null) {

      // Wait till the last possible moment before screen rendering to hide the UI
      yield return null;

      Canvas[] canvases = new Canvas[0];
      if (hideUI) {
        canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach(Canvas c in canvases) {
          c.enabled = false;
        }

        yield return new WaitForEndOfFrame();
      }

      Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
      ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
      ss.Apply();

      string screenshotPath = null;
      if (!string.IsNullOrEmpty(relativeFilePath) && !string.IsNullOrEmpty(fileSaveNameFormat)) {
        DateTime now = DateTime.Now;
        string suffix = String.Concat(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Millisecond);
        screenshotPath = string.Concat(Application.persistentDataPath, "/", relativeFilePath, "/", string.Format(fileSaveNameFormat, suffix));    
        
        System.IO.File.WriteAllBytes(screenshotPath, ss.EncodeToPNG());
        yield return null;

        float startTime = Time.time;
        while(!System.IO.File.Exists(screenshotPath)) {
            // Don't wait anymore than 5 seconds for the file to write
            if(Time.time - startTime > 5.0f) {
                // Handle some state here that will
                //  let you recover if the image isn't available
                yield break;
            }
            yield return null;
        }
      }

      // Wait one more frame just to be sure the file is finished writing.
      yield return new WaitForEndOfFrame();

      if (hideUI) {
        foreach(Canvas c in canvases) {
          c.enabled = true;
        }

        yield return new WaitForEndOfFrame();
      }

      
      callback(new ScreenCaptureResult(ss, screenshotPath));  
    }  
  }
}
