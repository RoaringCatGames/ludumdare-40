using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Twitter;

public class TwitterAPIComponent : MonoBehaviour
{
  private static readonly string PREFS_TWITTER_ACCESS_KEY = "TwitterAccessToken";

  public string twitterSecretJsonPath = "Data/twitter.secret";

  public FileImageComponent fileImageComponent;
  public Text tweetText;

  private string consumerKey;
  private string consumerSecret;


  private string pin;  
  private TwitterRequestToken requestToken;
  private TwitterAccessToken accessToken;
  private TwitterClient twitterClient;

  void Start()
  {
    LoadStoredAccessToken();
    TextAsset twitterSecret = CachedResource.Load<TextAsset>(twitterSecretJsonPath);
    string[] values = twitterSecret.text.Split('|');
    consumerKey = values[0];
    consumerSecret = values[1];

    if (String.IsNullOrEmpty(consumerKey) || String.IsNullOrEmpty(consumerSecret))
    {
      throw new Exception("CONSUMER KEY/SECRET ARE EMPTY");
    }

    twitterClient = new TwitterClient(consumerKey, consumerSecret);
  }

  public void ShareImage()
  {
    if (accessToken == null)
    {
      SetupAccess();
    }
    else
    {
      Logger.Log("Sending Tweet with image ", fileImageComponent.filePath);
      //TODO: Share Image
      byte[] imgBinary = File.ReadAllBytes(fileImageComponent.filePath);
      string imgbase64 = System.Convert.ToBase64String(imgBinary);
      StartCoroutine(twitterClient.PostTweetWithMedia(
        tweetText.text,
        imgbase64,
        accessToken.Token,
        accessToken.TokenSecret,
        this.OnTweetPosted));
    }
  }
  public void SetupAccess()
  {
    Logger.Log("SUP??");
    //StartCoroutine(Twitter.API.GetRequestToken(consumerKey, consumerSecret, new Twitter.RequestTokenCallback(this.ProcessRequestTokenResponse)));

    StartCoroutine(twitterClient.GenerateRequestToken(ProcessRequestTokenResponse));
  }

  private void ProcessRequestTokenResponse(bool success, string responseJson)
  {
    Logger.Log("Custom Request Token Response", responseJson);
    if (success)
    {
      requestToken = JsonUtility.FromJson<Twitter.TwitterRequestToken>(responseJson);
      Logger.Log(requestToken.oauth_token, requestToken.oauth_token_secret, requestToken.oauth_callback_confirmed);
      Application.OpenURL(twitterClient.GetAppAuthorizationUrl(requestToken.oauth_token));

      /// TODO: Prompt For PIN
    }
    else
    {
      Logger.Log("Error while getting RequestToken");
    }
  }
  private void ProcessAccessTokenResponse(bool success, string response)
  {
    if (success)
    {
      TwitterAccessToken token = JsonUtility.FromJson<TwitterAccessToken>(response);
      Logger.Log("Token", token);
    }
    else
    {
      Logger.Log("FAILED", response);
    }
  }

  private void OnTweetPosted(bool success, string response) {
    if(success) {
      Logger.Log("SUCCESS", response);
    }else{
      Logger.Log("FAILED", response);
    }
  }

  private void LoadStoredAccessToken()
  {

    string accessDetails = PlayerPrefs.GetString(PREFS_TWITTER_ACCESS_KEY);
    Logger.Log("Found String is: ", accessDetails);

    accessToken = TwitterAccessToken.FromString(accessDetails);
  }

  private void StoreAccessToken(TwitterAccessToken token)
  {
    PlayerPrefs.GetString(token.ToString());
  }


}