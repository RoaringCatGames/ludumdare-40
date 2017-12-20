using System;
using UnityEngine;

namespace Twitter {

  public class TwitterAPIComponent : MonoBehaviour {

    private static readonly string PREFS_TWITTER_ACCESS_KEY = "twitter-access-data";
    

    private TwitterAccessToken accessToken;
    void Start(){
      LoadStoredAccessToken();
    }

    public void SetupAccess(){
      
    }

    private void LoadStoredAccessToken() {

      string accessDetails = PlayerPrefs.GetString(PREFS_TWITTER_ACCESS_KEY);
      Logger.Log("Found String is: ", accessDetails);

      accessToken = TwitterAccessToken.FromString(accessDetails);
    }

    private void StoreAccessToken(TwitterAccessToken token){
      PlayerPrefs.GetString(token.ToString());
    }
  }
}