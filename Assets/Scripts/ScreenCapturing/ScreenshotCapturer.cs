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
    twitterAnimator = twitterUI.GetComponent<Animator>();
  }
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
    string suffix = String.Concat(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Millisecond);
    string screenshotPath = "calligratree-" + suffix + ".png";
    ScreenCapture.CaptureScreenshot(screenshotPath, 2);
    yield return null;
    
    FileImageComponent fileImage = twitterUI.GetComponentInChildren<FileImageComponent>();
    fileImage.ApplyImage(screenshotPath);

    twitterAnimator.Play("twitter-ui-enter");

    // Show UI after we're done
    GameObject.Find("ResetLevelUI").GetComponent<Canvas>().enabled = true;
  }

  public Texture2D LoadTexture(string FilePath)
  {

    // Load a PNG or JPG file from disk to a Texture2D
    // Returns null if load fails

    Texture2D Tex2D;
    byte[] FileData;

    if (File.Exists(FilePath))
    {
      FileData = File.ReadAllBytes(FilePath);
      Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
      if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
        return Tex2D;                 // If data = readable -> return texture
    }
    return null;                     // Return null if load failed
  }

  //  IEnumerator ShareImageToTwitter() {
  // 		Dictionary<string, string> parameters = new Dictionary<string, string>();
  // 		parameters ["status"] = "Tweet from Unity";
  // 		yield return StartCoroutine (Twitter.Client.Post ("statuses/update", parameters, Callback));
  // 	}

  // 	void Callback(bool success, string response) {
  // 		if (success) {
  // 			Twitter.Tweet tweet = JsonUtility.FromJson<Twitter.Tweet> (response);

  // 		} else {
  // 			Debug.Log (response);
  // 		}
  // 	}


  // 	void start() {
  //   byte[] imgBinary = File.ReadAllBytes(path/to/the/file);
  //   string imgbase64 = System.Convert.ToBase64String(imgBinary);

  //   Dictionary<string, string> parameters = new Dictionary<string, string>();
  //   parameters["media_data"] = imgbase64;
  //   parameters["additional_owners"] = "additional owner if you have";
  //   StartCoroutine (Twitter.Client.Post ("media/upload", parameters, MediaUploadCallback));
  // }

  // void MediaUploadCallback(bool success, string response) {
  //   if (success) {
  //     Twitter.UploadMedia media = JsonUtility.FromJson<Twitter.UploadMedia>(response);

  //     Dictionary<string, string> parameters = new Dictionary<string, string>();
  //     parameters["media_ids"] = media.media_id.ToString();
  //     parameters["status"] = "Tweet text with image";
  //     StartCoroutine (Twitter.Client.Post ("statuses/update", parameters, StatusesUpdateCallback));
  //   } else {
  //     Debug.Log (response);
  //   }
  // }

  // void StatusesUpdateCallback(bool success, string response) {
  //   if (success) {
  //     Twitter.Tweet tweet = JsonUtility.FromJson<Twitter.Tweet> (response);
  //   } else {
  //     Debug.Log (response);
  //   }
  // }


}
