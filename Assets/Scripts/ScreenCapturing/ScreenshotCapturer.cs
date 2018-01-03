using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Twitter;

public class ScreenshotCapturer : MonoBehaviour
{

  public GameObject twitterUI;

  private Animator twitterAnimator;
  void Start()
  {
    //twitterAnimator = twitterUI.GetComponent<Animator>();
  }
  public void OnClickScreenCaptureButton()
  {
    StartCoroutine(CaptureScreen());
  }
  public IEnumerator CaptureScreen()
  {
    // Wait till the last possible moment before screen rendering to hide the UI
    yield return null;
    BranchManager.instance.ToggleUIEnabled(false);

    // Wait for screen rendering to complete
    yield return new WaitForEndOfFrame();

    // Take screenshot
    DateTime now = DateTime.Now;
    string suffix = String.Concat(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Millisecond);
    string screenshotPath = "calligratree-" + suffix + ".png";
    ScreenCapture.CaptureScreenshot(screenshotPath, 1);
    if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
      screenshotPath = Application.persistentDataPath + "/" + screenshotPath;
    }
    yield return null;

    float startTime = Time.time;
    while(!System.IO.File.Exists(screenshotPath)) {
        // tried too long to save the file ?
        if(Time.time - startTime > 5.0f) {
            yield break;
        }
        yield return null;
    }
    yield return new WaitForEndOfFrame();
    FileImageComponent fileImage = twitterUI.GetComponentInChildren<FileImageComponent>();
    
    fileImage.ApplyImage(screenshotPath);

    BranchManager.instance.ToggleShareUI(true);
    //twitterAnimator.Play("twitter-ui-enter");

    // Show UI after we're done
    BranchManager.instance.ToggleUIEnabled(true);
  }
}
