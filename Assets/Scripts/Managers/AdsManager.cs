using System;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace RCGManagement {

  public enum AdWatchState {
    Watched,
    Skipped,
    Failed
  }

  public delegate void AdvertisementCallback(AdWatchState watchResult);

  public class AdsManager : MonoBehaviour {

    private static readonly bool USE_TEST_MODE = false;
    public static AdsManager instance;

    public string iosGameId;
    public string androidGameId;


    public void Start() {
      if(instance == null) {
        instance = this;

        #if UNITY_ADS
          string gameId;
          #if UNITY_IOS
            gameId = iosGameId;
          #elif UNITY_ANDROID
            gameId = androidGameId;
          #endif
          Logger.Log("Init Ads");
          instance._initialize(gameId);
        #endif

        DontDestroyOnLoad(instance);
      } else if (instance != this) {
        Destroy(this);
      }
    }

    private void _initialize(string gameId){
      #if UNITY_ADS
      if(gameId != null) {
        Advertisement.Initialize(gameId, USE_TEST_MODE);
      }                   
      #endif
    }

    public void ShowAd(AdvertisementCallback callback = null) {
      #if UNITY_ADS
      string placementId = "rewardedVideo";
      if(callback != null) {
        ShowOptions options = new ShowOptions();
        options.resultCallback = (ShowResult result) => {
          Logger.Log("Advertisement Result!");
          HandleShowResult(result, callback);
        };      
        Logger.Log("Firing Off Advertisement!");
        Advertisement.Show(placementId, options);
      }else {
        Advertisement.Show(placementId);
      }
      #endif
    }

    private void HandleShowResult(ShowResult result, AdvertisementCallback callback)
    {
      #if UNITY_ADS
      switch (result)
      {
          case ShowResult.Finished:
              Debug.Log("The ad was successfully shown.");
              //
              // YOUR CODE TO REWARD THE GAMER
              // Give coins etc.
              callback(AdWatchState.Watched);
              break;
          case ShowResult.Skipped:
              Debug.Log("The ad was skipped before reaching the end.");
              callback(AdWatchState.Skipped);
              break;
          case ShowResult.Failed:
              Debug.LogError("The ad failed to be shown.");
              callback(AdWatchState.Failed);
              break;
      }
      #endif  
    }  
  }
}