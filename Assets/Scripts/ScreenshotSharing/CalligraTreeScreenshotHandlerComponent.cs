using System;
using UnityEngine;
using LitterBox.Utils;

public class CalligraTreeScreenshotHandlerComponent: MonoBehaviour {

  public GameObject twitterUI;

  public void StartTwitterSharingProcess(Texture2D screenshot) {

    FileImageComponent fileImage = twitterUI.GetComponentInChildren<FileImageComponent>();
    
    fileImage.ApplyImage(screenshot);

    BranchManager.instance.ToggleShareUI(true);

    Kitten.Meow("Start Twitter Sharing Process FIRED!");
  }
  public void SaveToGallery(Texture2D screenshot) {
    #if UNITY_IOS || UNITY_ANDROID
    // Save to Gallery!
    NativeGallery.SaveToGallery(screenshot, "CalligraTrees", "calligratree-{0}.png");
    
    #endif
    Kitten.Meow("SAVE TO GALLERY FIRED!");
  }
}