using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotCapturer : MonoBehaviour {

 public void OnClickScreenCaptureButton()
 {
    StartCoroutine(CaptureScreen());
 }
 public IEnumerator CaptureScreen()
 {
    // Wait till the last possible moment before screen rendering to hide the UI
    yield return null;
    GameObject.Find("ResetLevelUI").GetComponent<Canvas>().enabled = false;
 
    // Wait for screen rendering to complete
    yield return new WaitForEndOfFrame();
 
    // Take screenshot
		DateTime now = DateTime.Now;
		string suffix = String.Concat(now.Year, now.Month, now.Day, now.Hour,now.Minute, now.Millisecond);
    ScreenCapture.CaptureScreenshot("calligratree-" + suffix + ".png", 2);
		yield return null;
 
    // Show UI after we're done
    GameObject.Find("ResetLevelUI").GetComponent<Canvas>().enabled = true;
 }
}
