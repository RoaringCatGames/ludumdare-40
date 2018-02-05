using System;
using UnityEngine;
using LitterBox.ScreenCapture;

[RequireComponent(typeof(ScreenCaptureComponent))]
public class ScreenCaptureOverridesComponent : MonoBehaviour {

  public bool overrideFileSaveOnMobile = true;
  void Start() {
    #if UNITY_IOS || UNITY_ANDROID    
    if(overrideFileSaveOnMobile) {
      ScreenCaptureComponent scc = GetComponent<ScreenCaptureComponent>();
      scc.shouldSaveToFile = false;
    }
    #endif
  }
}